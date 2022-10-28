# Godot-Project: Broomstick-Runner
Broomstick Runner is a game that blends in Roguelike Elements with Infinite Runner Mechanics made using Godot 3.5

## Gameplay
In Broomstick Runner you assume the role of a Hedgewitch, flying around the world in order to gather ingredients, deliver potions or cast spells for her Clients. As such the main gameplay is split in three phases. The Planning Phase, the Running Phase and the Resting Phase.

### Planning Phase
During the Planning Phase, the Player is given a Map of the Game World. Upon it he can see pinned his available quests, his workshop as well as locations from where he can collect some special ingredients. The world map is split into Regions, for which the player might know some or none information depending on how much they have travelled through each region. 
In this Phase, the Player can pick their destination and create their route towards it by placing waypoints all across the map. Using the line that is generated over the world map and collecting information from the World Map through its geometry, a runner level is generated. The size of the line would immediately affect the size of the level, while the region borders would split the level into different segments, each segment generated using the parameters of its affiliated region. 
Passing through different regions might provide the player with different difficulties or offer different collectibles for them to pick-up during the Running Phase.
The Purpose of this Phase is for the player to check their available quests and plan their routes accordingly in order to maximize their rewards. The player should take the following parameters under consideration when drawing their route. 
* __Quests' Expiration Date__: Every quest has a time limit. If it isn't completed by the that time, it expires and it becomes impossible to collect its rewards and perhaps it might have some other penalties as well.
* __Day/ Night Cycle__: Some regions might be more dangerous at night, or perhaps some ingredients can only be collected at certain times.
* __Regions__: Each Region offers its own parameters for the Runner Level generation.
* __Route Length__: The biggest the route, the more time it will take to complete it. 
* __Quest Requirements__: A quest might require delivering a certain item. Make sure that you have that item in your possession before going to the Delivery destination. This might require planning different routes first in order to collect the ingredients required and craft them into an item at your workshop.

### Running Phase
During this Phase, the Player has to go through the Runner Level Generated at the end of the Planning Phase. By controlling the Witch's movement in a Vertical Axis, they need to avoid the obstacles scrolling their way from the right side of the game window. The Player is given a Mana Gauge, which depletes when he casts spells. The Mana gauge is refilled by collecting Mana Crystal along the level. Spells are meant to help the player with completing their objective, so they might increase his speed, manipulate time, destroy obstacles or harm enemies.
By default, the player travels at a set speed. Colliding with obstacles, damages the Player and temporarily slows them down.
Some times, enemies might be generated into a Level. Unlike obstacles, enemies stick to the right side of the screen until they are killed. Enemies will generate new sets of obstacles, making the level harder for the player. 
If the Player's Health drops to 0, then they lose. They'd have to proceed to the Resting Phase immediately, updating their location in the world map based on the distance they have travelled through the level. Certain items like Timewrap Potions might allow trying the level from the start or a Checkpoint Spell might allow retrying from a certain fixed point in the route.
As the Player flies through the level, time progresses and quests might expire. If the player is in a Running Route for more than 24 in-game hours, then they are applied an Exhaustion State, which makes the game even harder for them (Double Damage Receive, Doubled Slow duration upon collision with Obstacle, Half Mana Gain from Crystals). 

### Resting Phase
The Player has completed their run. Either they completed the level, or they were forced to drop out, their position in the World Map is updated. Depending on that position they can check whether they can complete any quests in their vicinity. Then they proceed to rest. Time progresses by 8 hours and the Player's Health is recovered, while any Exhaustion States are removed. 
Different locations might offer special options during the Resting Phase. For example, in her Workshop, a Witch can craft items using the various ingredients in her collection, or research some new spells. 

#### More details and information regarding the various components of the game can be found in the Design Documents. 
