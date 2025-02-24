v(X) :- e(X, Y, W).
v(Y) :- e(X, Y, W).

{source(X)} :- v(X).
{target(X)} :- v(X).
:- source(X), source(Y), X != Y.
:- target(X), target(Y), X != Y.

{e_in(X, Y)} :- e(X, Y, W).

r(X, Y) :- e_in(X, Y).
r(X, Z) :- r(X, Y), e_in(Y, Z).

path(X, Y, L) :- e_in(X, Y), e(X, Y, L).
path(X, Z, L + W) :- path(X, Y, L), e_in(Y, Z), e(Y, Z, W).

:- source(S), e_in(X, Y), X != S, not r(S, X).

:- #count{X, Y : source(X), e_in(X, Y)} != 1.
:- #count{X, Y : target(Y), e_in(X, Y)} != 1.
:- source(Y), e_in(X, Y).
:- target(X), e_in(X, Y).
:- v(X), #count{X, Y : e_in(X, Y)} > 1.
:- v(Y), #count{X, Y : e_in(X, Y)} > 1.

:~ c(X, Y, Min, Max), not r(X, Y). [1, X, Y]
:~ c(X, Y, Min, Max), path(X, Y, L), L < Min. [1, X, Y]
:~ c(X, Y, Min, Max), path(X, Y, L), L > Max. [1, X, Y]
