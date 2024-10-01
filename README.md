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

### What needs to be done

Playeres need to be able to interact with chest objects which occupy certain spaces on the board.

A card gacha system needs to be implemented such that when a chest is opened a random card is returned. This system needs to weigh cards by their rarity to produce expected card drop rates.

</details>











<details>

<summary>Prototype Week 2</summary>
 
# Prototype 2 Objectives Status:


## Spell card Scriptable Object: **Implemented**

### What's been going well
The cards are implemented with the round manager to effectively create a system that can be expanded with relative ease

### What’s needs to be done

More cards need to be implemented to create move variety in player actions. Cards also need to have different effect types depending on if the card is an attack spell, or passive spell, or buff spell.

## Action Queue & Turn System: **Implemented**

### What's been going well

The turn systems seems to be functioning as intended with no immediate issues being presented. The code is implemented with the photon unity network such that it is online multiplayer capable.

### What needs to be done

There is no pacing in the execution of player actions. Player actions are meant to take place simultaneously but as it exists now they all complete instantaneously.

There needs to be implementation to show the order which players must act and to show who is acting and what effect their action had on other players.

## Player Characters: **implemented**
### What's been going well

players are fully capable of moving, attacking, taking damage, and dying. All of the basic functionality of players is implemented

### What needs to be done

Players now need to be able to pickup casting crystals which act as a resource for casting spells and attacking.

players need to have expandable hands of spells which change throughout playtime.

## Game UI: **In progress**

### What's been going well

the most integral functions of the ui such as health and the player controls are functional.

### What needs to be done

The ui needs to be cleaned up a little bit.

When directional casting is implemented there needs to be UI created to choose the direction of the cast.

## Movement System: **Implemented**

### What's been going well

Players can move in turn and their movement range is properly displayed.

### What needs to be done

A breadth first search system still needs to be implemented.

players need to be able to pickup casting crystals that will be placed on the board.

player must be able to interact with chest objects which occupy certain spaces on the board.

(Optional) Farther into development the grid will need to be compatible with a unique cards functionality which allows players to travers walls. The grid will need to hide certain grid spaces on top of walls until a player activates this ability, and hide these spaces once it is no longer in use.

## Create chest card gacha system: **Not Started**

### What needs to be done

Playeres need to be able to interact with chest objects which occupy certain spaces on the board.

A card gacha system needs to be implemented such that when a chest is opened a random card is returned. This system needs to weigh cards by their rarity to produce expected card drop rates.

</details>











<details>

<summary>Prototype Week 2</summary>
 
# Prototype 2 Objectives Status:


## Spell card Scriptable Object: **Implemented**

### What's been going well
No notable changes have been made to the structure of the cards.

### What’s needs to be done

More cards need to be implemented to create move variety in player actions. Cards also need to have different effect types depending on if the card is an attack spell, or passive spell, or buff spell.

## Action Queue & Turn System: **Implemented**

### What's been going well

Players actions play out in a approprietly timed out sequence that effectively communicates the events of each round and accurately keeps track of player status each round.

### What needs to be done

In a larger map there needs to be more dynamic camera control. Players should be able to see an large enough space around them to understand their surroundings but not so far to make it difficult to see the details.

## Player Characters: **implemented**
### What's been going well

Players are now capable of casting directional spells.

### What needs to be done

Players now need to be able to pickup casting crystals which act as a resource for casting spells and attacking.

players need to have expandable hands of spells which change throughout playtime.

players need to accurately keep track of their spell uses for each card.

## Game UI: **In progress**

### What's been going well

the UI controls for direction casting have been implemented.
### What needs to be done

The ui needs to be cleaned up a little bit.

## Movement System: **Implemented**

### What's been going well

no changes have been made to player movement this week.

### What needs to be done

A breadth first search system still needs to be implemented.

players need to be able to pickup casting crystals that will be placed on the board.

player must be able to interact with chest objects which occupy certain spaces on the board such as chests.

(Optional) Farther into development the grid will need to be compatible with a unique cards functionality which allows players to travers walls. The grid will need to hide certain grid spaces on top of walls until a player activates this ability, and hide these spaces once it is no longer in use.

## Create chest card gacha system: **Not Started**

### What needs to be done

Playeres need to be able to interact with chest objects which occupy certain spaces on the board.

A card gacha system needs to be implemented such that when a chest is opened a random card is returned. This system needs to weigh cards by their rarity to produce expected card drop rates.

</details>


