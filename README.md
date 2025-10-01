# Fuel Up
### Unity Version
Long Term Version Unity Editor 6000.0.50f1 was used with the Universal Pipeline Renderer. 
### Current Progress
A mostly complete vertical slice was finished. There are a few elements missing such as a video game manager to have a fully playable level. However, some of the larger pieces of architecture are complete which should make constructing future levels much quicker. 
### Stale Branch
The only other branch besides main (which is likely stale at the time of your reading this) is the unfinished code the minigame manager. It was in some working state for a single minigame but some testing with more than one minigame proved to not work. Whether that was due to event channels set incorrectly or improper code is unknown to me. The code in there also did not pass any review from the Coding Style Guide. It was attempted to work as a general class by having each individual game manager be an interface of minigame which then could call the appropriate functions.
