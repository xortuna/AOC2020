Nearly gave up on this one, the gold was tough.

Originally I had my Match functions returning a bool, but this meant that it could not check the other possiblities in OR statements.
E.g 

0: 1 3

1: 2 | 2 1

2: "a"

3: "b"

AAB

My logic would match "a" on the LHS of rule 1, this would return true, but the RHS of rule 1 would not be checked, so "aa" was never vistied.

By outputting all posibilties of OR's in rule 1, rule 0 could then use both A and AA, allowing it to match AAB
