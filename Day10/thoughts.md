This was a tough one, manly because my inital brute force solution for part two had way too many permutations and was taking too long to output an answer.

A rather embarising amount of time passed before I worked out a method to optimise the number of routes checked.
This was done by splitting on adaptors that only compatable with one previous adaptor then solving the indivdual sets of permutations then multipling all the sets together.

There is a rather obvious flaw in this method in that if all the adaptors are compatable with two or more then this optimisation is moot. But it worke well enough to grab that second star.

