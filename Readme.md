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

## 6. Conclusions

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