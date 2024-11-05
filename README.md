# LSC-Prototype

Build Link:  https://dubbep.github.io/LSC-Prototype/

<details>

<summary>Prototype Week 1</summary>
 
# Prototype Week 1 Objectives Status:


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
 
# Prototype Week 2 Objectives Status:


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

<summary>Prototype Week 3</summary>
 
# Prototype Week 3 Objectives Status:


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


<details>

<summary>Prototype Week 4</summary>
 
# Prototype Week 4 Objectives Status:


## Spell card Scriptable Object: **Implemented**

### What's been going well
Several new cards have been added and implemented totaling 7 functioning cards.

### What’s needs to be done
There needs to be more cards with a better variety of utility to create more interesting and varied gameplay. The Orb Of Confusion card needs to be fixed as well.

It would be ideal to have 16 cards implemented by next build.

## Action Queue & Turn System: **Implemented**

### What's been going well

A notification system has been implemented into the turn system to describe actions as they play out.

### What needs to be done

Spells that have a large range should be taken into account in the camera system. players should be able to see the whole range of their attack if it is large.

The notifications are difficult to read because of how fast they go by. This shoul be adjusted or changed such the the playe manually removes the text.

In a larger map there needs to be more dynamic camera control. Players should be able to see an large enough space around them to understand their surroundings but not so far to make it difficult to see the details.

## Player Characters: **Implemented**
### What's been going well

Players have pseudo statis affects such as being stunned or confused. As such the code for determining the effects of spells has been expanded. This system is currently convoluted and needs to be refined.

### What needs to be done

Players need to wait for a spells cooldown before repeated use to prevent spamming, and spells need to have a limited number of uses.

## Game UI: **Implemented**

### What's been going well
The players card hand can now dynamically add and remove cards.

### What needs to be done
The ui needs to be cleaned up a little bit.

### What needs to be done

A breadth first search system still needs to be implemented.

players need to be able to pickup casting crystals that will be placed on the board.

player must be able to interact with chest objects which occupy certain spaces on the board such as chests.

(Optional) Farther into development the grid will need to be compatible with a unique cards functionality which allows players to travers walls. The grid will need to hide certain grid spaces on top of walls until a player activates this ability, and hide these spaces once it is no longer in use.

## Create chest card gacha system: **Implemented**

### What's been going well
Chests exist on the map which players are able to interact with. When a player lands on a chest they recieve a random cards which is then added to their hand.

### What needs to be done
The system often gives players repeat cards due to the small card pool. Drop rates are also un tested so rates need to be adjusted and card need to be added to make this system more rebust

Instead of just giving a player a random card a menu should appear that gives the player a choice between two random cards.

</details>



<details>

<summary>Vertical Slice Week 1 & 2</summary>
 
# Vertical Slice Week 1 & 2 Objectives Status:

## Additions:

### UI
A wait timer now exists which limits the ammount of time players can take during their planning phase. There is also a new UI element which displays the players who have yet to finish their turn.

### Chests
A card selection screen when a chest is opened which provides the player with a choice of what card to aquire.

### Models
Models have been implemented for the casting crystals and chests.

### Camera
The camera now tracks to the general quadrant that the player is located on in the map.


## Plans
Implement a free camera mode that gives player direct control over the camera. \n
Implement UI elements to inform the player on the basic funtionality of the game. \n
Implement UI to display the turn order. \n
Create a new map model. \n
Create player piece models.
Add effects to spells so that they have visuals or animations.
Add sound effects to the attacks.
Create player classes which provide unique benefits and abilities.

</details>




<details>

<summary>Vertical Slice Week 3</summary>
 
# Vertical Slice Week 3 Objectives Status:

## Additions:

### UI
Info screen, and turn order UI element have been implemeneted.

### Models
The map layout has been expanded and blocked out. Once the mesh is exported the map geometry will be created.

### Camera
The free camera mode has been implemented.


## Plans
Create a new map model.
Create player piece models.
Add effects to spells so that they have visuals or animations.
Add sound effects to the attacks.
Create player classes which provide unique benefits and abilities.

</details>


<details>

<summary>Vertical Slice Week 4</summary>
 
# Vertical Slice Week 4 Objectives Status:

## Additions:

### Models
The new map model has been created and imported into the game scene. A simple player piece model has been created as well.

### Visual Effects
Particle effects and animations for each attack have been created. A script to play these animations during the game has been implemented as well.

## Plans
Add sound effects to the attacks.
Create player classes which provide unique benefits and abilities.

</details>
