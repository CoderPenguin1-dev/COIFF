# Cinder's Own Interactive Fiction Framework
COIFF is a basic engine made for "choose-your-own-adventure" style games (like gamebooks).

The framework is split into two main parts, the Engine and the Frontend.
* The [Cinder Engine](CinderEngine) runs the game's logic.
* The [Flame](Flame) frontend shows data to the player and allows them to actually make choices.

Cinder Engine itself was designed in a way so a frontend could be easily written for it without the engine being dependant on it.