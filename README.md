# LSC-Prototype

Build Link:  https://dubbep.github.io/LSC-Prototype/

<details>

<summary>Prototype Week 1</summary>
 
# Prototype 1 Objectives Status:


## Spell card Scriptable Object: **In progress**

### What's going well

The scriptable object template has been created. 

The template holds all of the basic information that a card needs.

There is a display script that takes the Card information and displays it on a UI game object in the scene. (Not in current build)

### What’s needs to be done
The cards need to have behaviors that execute their functionality. 

The biggest hurdle with this is organizing this such that it approprietly interacts with the other systems.

Most cards will need a effect range which encompasses a certain number of board spaces relative to the players position.

Each Card that has an effect range needs a unique algorithm to build out those effect ranges. The cards also need a function for their actual effects.

This information has to be sent to a queue which holds all player actions for the round. The queue orders them by their delay value and executes them in that order.

Non attack spells need to be able to implement their unique functionality too such as altering player health or movement range.

spells also need to keep track of and play their visual effects.

## Action Queue & Turn System: **Not Started**

### What needs to be done
The game has a turn system where each player takes an action (moving, cast spell etc.) which needs to be put into a queue or list and then executed in the scene according to the delay of the action such that the action with the shortest delay will be executed first. Once all actions are executed the round ends, a new round begins, and players take actions once more.

Additional implementation such as attacks being interrupted and thus being removed from the queue, actions playing at a reasonable speed rather than being executed instantaneously etc.

## Player Characters: **In progress**
### What's been going well
I’ve largely Identified the information that players need to hold and what methods they need to have to function. These methods just need to be implemented.

### What needs to be done
The most important methods which need to be implemented are:
PrepareCast();
CastSpell();

## Game UI: **In progress**

### What's been going well

Basic attack and move buttons have been added.

move button functionality has been implemented.

### What needs to be done

Once players are capable of casting spells and preforming attacks the attack button needs to be linked

UI to hold information about the player character such as health.

UI to show the Cards a player currently holds.

## Movement System: **In progress**

### What's been going well

A grid system has been implemented which allows for characteres to travel the board via mouse clicks.

The grid system is flexible enough to be customized into unique shapes and is detatched from level geometry. This will allow for unique and interesting environments to be created that don't impact the game board.

Movement range has also been highlighted so that players can see the spaces they are allowed to travel.


### What needs to be done

A breadth first search system needs to be implemented so that characters will calculate the routes to their travel destination through the grid. This will allow for player to animate their movement rather than teleport.

(Optional) Farther into development the grid will need to be compatible with a unique cards functionality which allows players to travers walls. The grid will need to hide certain grid spaces on top of walls until a player activates this ability, and hide these spaces once it is no longer in use.

## Create chest card gacha system: **Not Started**
What needs to be done

Playeres need to be able to interact with chest objects which occupy certain spaces on the board.

A card gacha system needs to be implemented such that when a chest is opened a random card is returned. This system needs to weigh cards by their rarity to produce expected card drop rates.

</details>

