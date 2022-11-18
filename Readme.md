# Railway Programming: la voie vers un code plus honnête

**Railway programming: the path toward a more honest code**

## 1. Introduction

Concepts presented in this repository are techno-agnostic. Those may be idiomatic to the programming language you're used to. Whether they are already integrated or not, they remain easy to code.  

If you're familiar with Theory Category, you must know those concepts have specific names (functors, monads...). We chose on purpose to not use them during our talk as it is not a course about monads.   

## 2. Initial domain

We use as example codebase the MaitreD kata often used by [Mark Seemann](https://twitter.com/ploeh).  
It is a simple API used to validate and register reservations for a restaurant.  

When looking at signatures, we can guess the presence of some side effects (usage of `Task` and `void`), but without further details.  
This is symptomatic, to know all possible behaviors, you have to either:
- open and read code
- run tests against the system to explore behaviors

You may have missed this, but the name of the reservation is optional.  

## 3. Make it explicit

## 4. Interlude: lists

## 5. Railway programming

## 6. Conclusions

## Resources

### Scott Wlaschin
- [Scott's Twitter](https://twitter.com/ScottWlaschin)
- [Domain Modeling Made Functional](https://pragprog.com/titles/swdddf/domain-modeling-made-functional/)
- [Railway Oriented Programming](https://fsharpforfunandprofit.com/rop/)

### Mark Seemann's blog posts
- [Asynchronous Injection](https://blog.ploeh.dk/2019/02/11/asynchronous-injection/)
- [Impureim sandwich](https://blog.ploeh.dk/2020/03/02/impureim-sandwich/)

### To go further on theory
- [Théorie des Categories: vous la connaissez déjà (French)](https://youtu.be/DFZ7arg1XFc)