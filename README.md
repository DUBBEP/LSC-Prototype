# LSC-Prototype

Build Link:  https://dubbep.github.io/LSC-Prototype/
 
# Prototype 1 Objectives Status:


## Spell card Scriptable Object: ** In progress **

### What's going well

The scriptable object template has been created. 

The template holds all of the basic information that a card needs.

There is a display script that takes the Card information and displays it on a UI game object in the scene.

### What’s needs to be done
The cards need to have behaviors that execute their functionality. 

The biggest hurdle with this is organizing this such that it approprietly interacts with the other systems.

Most cards will need a effect range which encompasses a certain number of board spaces relative to the players position.

Each Card that has an effect range needs a unique algorithm to build out those effect ranges. The cards also need a function for their actual effects.

This information has to be sent to a queue which holds all player actions for the round and orders them by their delay value and executes them in that order.

Non attack spells need to be able to implement their unique functionality too such as altering player health or movement range.

spells also need to keep track of and play their visual effects.

## Event Queue Turn System: ** Not Started **

### What needs to be done
The game has a turn system where each player takes an action (moving, cast spell etc.) which needs to be put into a queue or list and then executed in the scene according to the delay of the action such that the action with the shortest delay will be executed first. Once all actions are executed the round ends, a new round begins, and players take actions once more.

Additional implementation such as attacks being interrupted and thus being removed from the queue, actions playing at a reasonable speed rather than being executed instantaneously etc.

## Player Characters: ** In progress ** 
### What's been going well
I’ve largely Identified the information that players need to hold and what methods they need to have to function. These methods just need to be implemented.
What needs to be done
The methods which need to be implemented are:
PrepareCast();
CastSpell();

## Game UI: ** In progress **
What's been going well
What needs to be done
Movement System: In progress
What's been going well
What needs to be done
Create chest card gacha system: Not Started
What needs to be done
