# Overview
A C# Implementation of Functional Lenses, but adapted to Object-Oriented Programming.
## License
MIT
## Dependencies
⦁   Net Standard 2.1
## Motivation
I always find complexity when updating nested objects, especially when dealing with nested collections. Common OOP Patterns like the Builder Pattern and some
other tricks work, but they are not good enough because:
1. They cause coupling
2. Writing builders consume time.

Manipulating data structures can be challenging, but Functional Programming offers an elegant solution through the use of Lenses. A Lens enables you to focus on a specific part of a data structure, allowing you to get or set values without having to consider the entire structure.

This library applies some of those concepts to the Object Oriented Programming world, allowing you to update objects with a lens that is generic and compsoable.

It also works great with immutable records, as demonstrated in the tests.

You can:
1. Update an object property.
2. Update a nested object property.
3. Update a collection of objects.
4. Update a nested collection of objects.
5. Update a property on an object inside a nested collection

There is no limit to the depth of the nested objects.


More on the [Wiki](https://github.com/juliocachaydev/application.core.optics/wiki)
## Credits
Author: Julio C. Cachay. Chattanooga, TN, USA.

This library is inspired by the concept of lenses in functional programming, and in the [optics.ts](https://akheron.github.io/optics-ts/) library