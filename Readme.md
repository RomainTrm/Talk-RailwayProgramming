# Railway programming: the path toward a more honest code

## 1. Introduction

Concepts presented in this repository are techno-agnostic. Those may be idiomatic to the programming language you're used to. Whether they are already integrated or not, they remain easy to code.  

If you're familiar with Theory Category, you must know those concepts have specific names (functors, monads...). We chose on purpose to not use them during our talk as it is not a course about monads.  

Implementations shown in this repository are just one of the many ways of coding such concepts.

## 2. Initial domain

We use as example codebase the MaitreD kata often used by [Mark Seemann](https://twitter.com/ploeh).  
It is a simple API used to validate and register reservations for a restaurant.  

When looking at signatures, we can guess the presence of some side effects (usage of `Task` and `void`), but without further details.  
This is symptomatic, to know all possible behaviors, we have to either:
- open and read code
- run tests against the system

You may have missed this, but the name of the reservation is optional : `name ?? ""`.  

## 3. Make it explicit

### Optional values

We can explicit optional values with a dedicated type `Option<T>`/`Maybe<T>`.  
This way, `RegisterReservationCommand` constructor signature evolves from:  
`(DateTime At, string Email, int Quantity, string Name)`  
to   
`(DateTime At, string Email, int Quantity, Option<string> Name)`.

### Errors

To explicit possible errors, we use the `Result<TValue, TError>` type, replacing exceptions with an `Error<TValue, TError>(TError Value)`.    

As we're now using `Result`, we have to specify a return type for the happy path, even for methods that were returning `void` or `Task` (an asynchronous operation without any return).
To do so, we used the `Unit` type, it's just an empty type with structural equality that act as a placeholder.  

Public signature is now: `Task<Result<Unit, Errors>> RegisterReservation(RegisterReservationCommand command)`.  
Note, error path are now easier to test as we're not trying to catch exceptions, rather comparing results thanks to structural equality.   

### Composition issues

Initial version of the domain was many focus on happy path. Now we've explicit errors, we're facing composition issues.  
We must either:
- extract and return the error
- extract the value and continue processing

For now, we'll use an imperative solution, easy enough to understand even if it's rather disappointing and it adds a lot of noise into our code.  
It takes the following form:  
```C#
Result<xxx, Errors> xxxResult = ...

if (xxxResult is Error<xxx, Errors> xxxError) return new Error<Unit, Errors>(xxxError.Value);
var xxx = ((Ok<xxx, Errors>)xxxResult).Value;
```

## 4. Interlude: lists

As mentioned on part 3, we're facing issues to manipulate content of types like `Result<TValue, TError>`.  
Fortunately, there is a generic type we're used to which have solved similar issues: lists.  

To manipulate them, we're used to the basic and universal building block: `for loops`. But those are kinda noisy, and tend to mix iteration logic with relevant domain logic (as we did with results on previous part).  
That's why most languages/frameworks now includes dedicated methods that abstracts iteration logic and only requires domain logic as input. Here's some examples:
- `map`, `Select`
- `filter`, `Where`
- `bind`, `flatMap`, `SelectMany`
- `reduce`, `fold`, `Aggregate`
- etc...

Now, let's think:   
- Isn't the `Option` type just a list containing either nothing or exactly one element?
- `Result` isn't the same as `Option`, just replacing empty with another dedicated value? 

So, if such methods exists for lists, we may manage to code them for `Option` and `Result`.

## 5. Railway programming

Railway programming is a metaphor popularized by [Scott Wlaschin](https://twitter.com/ScottWlaschin).  
In this metaphor, pure methods are represented as railways.  

A simple function is represented as a simple line:  
```text
'T1 -Method-> 'T2
```

And you can link railways to compose functions:
```text
'T1 -Method-> 'T2 -Method-> 'T3 
```

Some methods have diverging return types that can be represented as two different paths/railways (`Option`/`Result`):  
```text
'T1 -Method-> Ok 'T2            --Happy path
         \
          \-> Error 'TError     --Error path
```

### Map

But how to compose? 
```text
'T1 -Method1-> Ok 'T2           and       'T2 -Method2-> 'T3
          \
           \-> Error 'TError
```

We have a mismatch between `Ok 'T2` as output of `Method1` and `'T2` as input of `Method2`. Also we have no behavior specified if an error is returned by `Method1`.  
This is the role of `map` method to transform 
```text
'T2 -Method2-> 'T3  
```
to 
```text
Ok 'T2 -------Method2-> Ok 'T3

Error 'TError --------> Error 'TError
```

Then we can compose
```text
'T1 -Method1-> Ok 'T2 --------> map Method2-> Ok 'T3
          \
           \-> Error 'TError ---------------> Error 'TError 
```

### Bind

But how to compose?
```text
'T1 -Method1-> Ok 'T2           and       'T2 -Method2-> Ok 'T3
          \                                         \
           \-> Error 'TError                         \-> Error 'TError
```

We have a mismatch between `Ok 'T2` as output of `Method1` and `'T2` as input of `Method2`. Also we have no behavior specified if an error is returned by `Method1`.  
This is the role of `bind` method to transform
```text
'T2 -Method2-> Ok 'T3  
          \
           \-> Error 'TError
```
to
```text
Ok 'T2 -------Method2-> Ok 'T3
                   \
Error 'TError --------> Error 'TError
```

Then we can compose
```text
'T1 -Method1-> Ok 'T2 --------> bind Method2-> Ok 'T3
          \                               \
           \-> Error 'TError ----------------> Error 'TError 
```

### Impureim sandwich

We've just saw how to compose types like `List`, `Result` or `Option`. Unfortunately such types does compose poorly.    
In our example we have such a case with `Result` and `Task`: the main method should return `Task<Result<Unit, Errors>>`, but at some point we want to apply a method to the value of a `Result`. It means we'll get a `Result<Task, Errors>`.   

We want to invert these "container objects", to transform `Result<Task, 'TError>` to `Task<Result<Unit, 'TError>>`.  
Here's two strategies (see`TaskExtensions.cs` for implementation details):  
- specific to C# `Task<>` as dedicated keywords are available, a method `Task DoAsync(Result<'TValue, 'TError>, Func<'TValue, Task>)`
- a more generic method `Task<Result<'TResult, 'TError>> Traverse<'TResult>(Result<'TValue, 'TError>, Func<'TValue, Task<'TResult>>)`

Note the second method can be implemented for various types like `Result` and `List` or `Option` and `List`.  

These methods tends to mitigate this issue. But there is another common strategy used by functional programmers: impureim sandwich.  
The idea is to separate IO from Pure logic, and then to stack these "layers" of logic like a sandwich like 
```text
IO logic
Pure logic
IO logic
```

We follow this pattern in our final implementation.  

## 6. Conclusions

We discussed how to:  
- explicit with types details of the system (errors, mandatory/optional values)
- how to solve composition issues 
- how to deal with IO

It may seems more complicated than the code you're used to, but with practice, basic composition issues turn to be natural and won't need too much attention.

About errors: we would recommend you to distinct business errors from panic mode errors.    
Use `Result` to return business errors or errors you're interested by. Exceptions are fine as a panic mode, like "database is unreachable for whatsoever reason", in such a case, you will not try to handle it as it is unrecoverable.  

Some languages provides powerful idiomatic syntaxes known as "for comprehension" or "computation expression". You can see an example using linq syntax in `ComputationExample.cs`.  

## Resources

### Scott Wlaschin
- [Scott's Twitter](https://twitter.com/ScottWlaschin)
- [Domain Modeling Made Functional](https://pragprog.com/titles/swdddf/domain-modeling-made-functional/)
- [Railway Oriented Programming](https://fsharpforfunandprofit.com/rop/)

### Mark Seemann's blog posts
- [Mark's Twitter](https://twitter.com/ploeh)
- [Asynchronous Injection](https://blog.ploeh.dk/2019/02/11/asynchronous-injection/)
- [Impureim sandwich](https://blog.ploeh.dk/2020/03/02/impureim-sandwich/)

### To go further on theory
- [Théorie des Categories: vous la connaissez déjà (French)](https://youtu.be/DFZ7arg1XFc)
