#### Goat Galaxy Games presents...

# Fluffed Up

1\. Start Scene File: Main Menu

2.

- Hit “Start Game” button from the first screen
- Select either one of the characters to play. The left player has a sword to attack, and the right player has a gun to shoot.
- Once the character is selected, the wave will start. Click left to attack enemies, and use WASD keys to navigate. Also, hold the right click to magnify the view.
- When all enemies are killed, an item shop will be available. Look for the green light flashing spot and hit the key “E” nearby to enter the shop mode. Click on the cards to upgrade the player stat or purchase health using in-game currency.
- Dodge-based upgrades like _roll, dash, and blink_ can be activated using **Left Shift** once purchased
- Mouse sensitivity settings can be accessed in the settings menu
- Currently, Gamepads are not supported. Sorry gamers. :(

3\. Manifest of which files authored by each teammate

- Sha Jackson
  - Player controls and mechanics
    - \[Script/Asset\] InputMap.cs/PlayerInput
    - \[Script\] (parts of) PlayerController.cs
      - Grounded Check
      - Coin Flush Handling (side coin counter that aggregates recent coin changes before adding them to the players total coin count)
      - Camera Relative Movement Control
      - Rotate Player With Camera
      - Dodge Controls
      - Some shooting controls
        - Hitscanning with raycast
        - Bullet travel time simulation
        - Reload & chambering mechanics
  - Third Person Camera Control
    - \[Asset\] PlayerAimCamera
    - \[Asset\] PlayerFollowCamera
    - \[Script\] ThirdPersonCameraController.cs
  - UI Display & Scripting
    - \[Assets & Script\] Stats display (in top left corner, scripted in PlayerController.cs)
    - \[Asset\] Crosshair
    - \[Asset & Script\] Ammo (PlayerController.cs)
    - \[Script\] Coin flush (PlayerController.cs)
    - \[Asset\] ShopUI
    - \[Scene\] Settings
    - \[Script\] PlayerSettings.cs
  - Animation
    - Alternating melee strikes
    - Running animation
    - Avatar masking for simultaneous running and striking
    - Roll & Dash
  - Shop & Upgrades
    - \[Scripts\] ShopController.cs & DropTables.cs
      - Random item selection and stocking
      - Upgrade purchasing
      - Drop rate configs
    - Upgrade implementation
      - \[Script\] Upgrade.cs
      - \[Script\] Skill.cs & ISkill.cs
        - RollSkill.cs
        - DashSkill.cs
        - BlinkSkill.cs
      - \[Script\] StatUpgrade.cs
        - Attack power, defense, attack/reload speed, move speed, health
      - \[Script\] GameModification.cs
        - Restock Upgrade
      - \[Script\] PlayerModification.cs
        - Flesh Wound Upgrade
  - Wave Scaling
    - \[Script\] (parts of) WaveManager.cs
      - Scaling functionality
        - \[Method\] ScaleEnemyDifficulty
      - Random spawning scaled with difficulty
        - \[Method\] ComputeOnslaught
        - \[Method\] OnslaughtMinon (and util functions)

Computes random waves of enemies based on a progressing difficulty score and predefined enemy difficulty scores using the unbounded knapsack algorithm

- - - - Enemy spawning trickle effect (enemies spawning in groups over time)
                - \[Method\] EnemyGroupHandler
                - \[Method\] EnemyLoader
    - And a lil' bit o' bug squashin' here 'n' there
- Taesung Heo
  - Top-level features
    - Wave spawn infrastructure
      - Defined WaveManager class to manage wave activations along with user game play
      - Define how the enemies will spawn for each wave.
      - Enemy spawn area defined in the map
    - Enemy Prefabs and AI Implementation
      - Added two enemies from RPG Monster DUO PBR Polyart set
      - For each enemy, refined animator state machines
      - Enemy AI implemented using NavMeshAgent to track the player
      - StateMachine implemented for enemies to switch between player tracking mode and attacking mode
      - Each enemy can be configured differently in the state machine to act differently
    - Enemy and Player Interaction
      - Event-based trigger to interact between enemy and the player
      - All attacks and damages are cascaded by events
    - Interactive shop lights
      - Implemented flashing green light to guide the player to the shop between waves.
    - Sound Effects
      - Background music player added
      - Composed the BGM music
      - Attack and damage sound effects added and linked to the events to be triggered.
      - Sound effects are played as 3D at a specific location where the event occurred.
  - Added Assets:
    - \[Asset - Prefab\] SlimePBR_AI
    - \[Asset - Prefab\] TurtleShellPBR_AI
    - \[Asset - Prefab\] Sound3D
    - \[Asset\] WaveManager
    - \[Asset\] EnemySpawnArea
    - \[Asset\] ShopIndicatorLight
    - \[Asset\] ShopBeaconLight
    - \[script\] BGMPlayer.cs: Plays background music
    - \[script\] EnemySlime.cs: Defines behaviors for slime enemy
    - \[script\] EnemyTurtle.cs: Defines behaviors for turtle enemy
    - \[script\] LightPulse.cs: Defines flashing light actions
    - \[script\] Sound3D.cs: Script to help with Sound3D prefab actions
    - \[script\] EnemyBase.cs: Defines common behaviors for enemies
    - \[script\] WaveManager.cs: Wave infrastructure system, backbone of event creation/listener registration of all player and enemies attack/damage. Wave definitions and spawns live here.
- Danh Le
  - Top-level features
    - UI
      - Health Bars
        - Created the UI components
        - Functionalities to update health when healed or damaged
        - The health bar is used by the player and enemies.
        - Added functionalities for updating health when healed or damaged.
        - For enemies, health bar always faces the player’s camera
      - Health Pack Count Display
        - Created UI components to show the number of health packs.
        - Functionality to update count.
      - Coins Collected Count Display
        - Created UI components to show the number of coins collected.
        - Functionality to update count.
      - Current Wave Display
        - Created UI components to show the current wave number.
      - Pause Screen
        - Resume, Restart, and Quit functionalities
    - Damage notifier
      - Added UI and logic for displaying the amount of damage the player inflicts on an enemy.
    - Interact Functionality
      - Added UI and logic to show if an object can be interacted with. Current capability involves triggering shops.
    - Healing Functionality
      - Handling logic and input to heal players if they have health packs available to use.
    - Shooting Character functionality
      - Improve shooting character, allowing them to shoot in any direction.
      - Fix character’s gun positioning.
    - Environment
      - Added new assets to expand the environment which includes the desert, forest, and winter biomes.
    - Player animations
      - Added a death animation for the player when they die.
    - Lighting
      - Improve lighting so the game isn’t dark. Also remove fog from the scene to see more distances in the environment.
    - Enemies
      - Created two new enemies, the Manta Ray and Mosquito enemies.
      - Updated their animators to react accordingly when getting hit, hitting the player, and dying.
  - Assets Implemented
    - \[Asset\] Dropped Items
      - Added assets for collectable items.
    - \[Asset\] LowPoly Environment Pack
      - The landscape or setting of our game, which includes mountains, various rocks and stones, and plants.
    - \[Asset\] Power-Ups and Dropped Collectable Items
      - A set of collectable items that our player can consume. The only supported item from this asset is the health item which is used as a consumable to heal the player.
    - \[Asset\] Low-Poly Medieval Market
      - Using the weapon market with objects. Acts as our shop that our player can upgrade their skills and purchase consumables at.
    - \[Asset\] Real Stars Skybox
      - Sets the outer setting of our game, which is a night sky to fit our outer-space setting.
    - \[Asset\] Monster Minion Survivor PBR Polyart
      - Assets for the Manta Ray and Mosquito enemies
    - \[Asset\] Atom Rocket Model
      - Asset for character’s space rocket
    - \[Asset\] Low Poly Forest Pack Winter
      - Asset for Winter Forest biome
    - \[Asset\] Low Poly Simple Nature Pack
      - Asset for Nature Forest biome
  - C# Script Files
    - \[script\] Billboard.cs
      - Used for the enemy’s health and assures that enemy health bars follow the player’s camera.
    - \[script\] CollectibleItem.cs
      - Allows for collectable items. When a player triggers it, they collect the item, which gets added to their inventory.
    - \[script\] FloatingText.cs
      - Used to display enemy damage notifier. However much damage the player deals to the enemy, that number will float above the enemy and disappear.
    - \[script\] HealthBar.cs
      - General class to manage health functionalities.
    - \[script\] InteractableObject.cs
      - General class to manage player interactions to objects. Currently only supports opening the shop.
    - \[script\] CharacterClass.cs
      - General class for players and enemies to inherit. They can contain common functionalities and attributes, which provides a cleaner code structure.
    - \[script\] PlayerController.cs
      - Manage how the player interacts with collectables, updates counters on UI, and self healing.
    - \[script\] EnemyMantaRay.cs
      - Used to handle AI State Machine for Manta Ray enemy.
    - \[script\] EnemyMosquito.cs
      - Used to handle AI State Machine for Mosquito enemy.
- June Kim
  - Top-Level Features:
    - UI
      - Main Menu Screen
        - Created UI components serving as main menu for game with ability to press Play Button and Quit Application
      - Select Character Screen
        - Created UI components for user’s ability to select character
      - Pause Screen
        - Created UI components for user’s ability to Resume, Restart Level, Quit to Menu
        - Logic to show Pause Screen when Esc key pressed
      - Death Screen
        - Created UI components for Death Screen that appears after the player dies in game. Allows for restarting game or going back to main menu
      - Second Character Capability
        - Made adjustments to first character and added gun weapon and complementary sound effect to create second character with implemented shooting capability
      - Tutorial Scene
        - Created tutorial UI components using Figma for user’s ability to learn controls and objectives for the game
      - Shop
        - Added UI components such as text and feedback messages for user to facilitate interaction with shop
  - Assets Implemented
    - Shop Purchase Sound
      - Plays confirmation sound as user feedback for when potion or upgrade is purchased from the shop
  - C# Script Files
    - ShopController.cs
      - Added implementation for feedback messages and sound to occur when shop items are interacted with
- Sahar Nikhah
  - Top Level Features:
    - MiniMap
      - Created a miniMap that can help navigate the player and enemies
      - Created prefabs and materials
      - Created the UI
      - Implemented algorithms, layers and tags
    - Environmental Hazards
      - Added environmental hazards that made the environment more interactive including:
      - Added snowman
      - Added cacti
      - Added traps
    - Shooting Skill
      - Implemented Shooting Skill functionality for player and created projectile prefab and spawn point and added animation
    - UI
      - Added Mini Map canvas
      - Added MiniMap Image rendering
      - Contributed to shop UI
  - Asset Added
    - - Basic PRG Icons
        - Cactus Package
        - Real Stars
        - Free Snowman
        - \[prefabs\] added markers to players prefab
        - \[prefabs\] environmental Hazards
        - \[prefabs\] MiniMap Camera
        - \[prefabs\] Projectile
  - Scripts
    - - Added EnvHazard.cs
        - Added Projectile.cs
        - Added EnvHazard.cs
        - Contributed to the playercontroller.cs
        - Contributed to other player script
        - MiniMap scripts
        - Contributed to Droptables.cs

4\. References

- Kaimira Weighted List: <https://github.com/cdanek/KaimiraWeightedList>
- Kenney Crosshair Pack: <https://kenney.nl/assets/crosshair-pack>
- Dog Knight PBR Polyart: <https://assetstore.unity.com/packages/3d/characters/animals/dog-knight-pbr-polyart-135227?srsltid=AfmBOoqtlCB2ffgDMOGnVaAp9UHkvKqChGsixq2gsAeNLFufSFX8Xm9E>
- LowPoly Environment Pack: <https://assetstore.unity.com/packages/3d/environments/landscapes/lowpoly-environment-pack-99479>
- Power-Ups and Dropped Collectable Items: <https://assetstore.unity.com/packages/3d/props/ten-power-ups-217666>
- Low-Poly Medieval Market: <https://assetstore.unity.com/packages/3d/environments/low-poly-medieval-market-262473>
- Real Stars Skybox: <https://assetstore.unity.com/packages/3d/environments/sci-fi/real-stars-skybox-lite-116333>
- RPG Monster DUO PBR Polyart: <https://assetstore.unity.com/packages/3d/characters/creatures/rpg-monster-duo-pbr-polyart-157762>
- Space Game Background for Menu Scenes: <https://www.istockphoto.com/vector/planets-and-nebula-background-in-pixel-art-style-gm1403514917-456051805>
- Monster Minion Survivor PBR Polyart: <https://assetstore.unity.com/packages/3d/characters/creatures/monster-minion-survivor-pbr-polyart-269515>
- Atom Rocket Model: <https://assetstore.unity.com/packages/3d/vehicles/space/atom-rocket-model-140021>
- Low Poly Forest Pack Winter: <https://assetstore.unity.com/packages/3d/environments/landscapes/lowpoly-forest-pack-winter-129565>
- Low Poly Simple Nature Pack <https://assetstore.unity.com/packages/3d/environments/landscapes/low-poly-simple-nature-pack-162153>
- Sound Effects: <https://pixabay.com/sound-effects/>

<https://assetstore.unity.com/packages/audio/sound-fx/shooting-sound-177096>

<https://assetstore.unity.com/packages/audio/sound-fx/rpg-essentials-sound-effects-free-227708>

- Stat icons: <https://game-icons.net/>
- Very helpful unity tutorials: <https://www.youtube.com/@CodeMonkeyUnity>
- Other very helpful unity tutorials: <https://www.youtube.com/@davegamedevelopment>
- Base structure for Unbounded Knapsack algorithm code: <https://neetcode.io/problems/coin-change-ii>