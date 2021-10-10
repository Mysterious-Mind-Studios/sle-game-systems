# SLE Game Systems API

The SLE Game Systems is a pack of APIs made to work with Unity® Engine with the main focus on performance by defaulft.

   Totally inspired in the ECS api developed by Unity's team, this system is architectured to work based on the data 
of it's components. Each component is handled <i>only</i> and <i>exclusivelly by the <b>system</b> that needs them</i>, each system works
independent of each other and runs just before any other user scripts (MonoBehaviours). This is a requirement needed 
to prevent lots of errors (such as NullReferenceException for example) and make your game runs smoothly. 
   All the data of the components is updated using the high performance Burst compiler and C# Jobs system provided by Unity, 
what means, that the systems run most of it's tasks in a multithreaded way, taking advantage of a parallelized high speed data processing.

<b> - Symplicity by default - </b>

   Despite it's design internally is focused on data, for users, it doesn't feel like this. Using any of the SLE Game Systems feels like
using any normal MonoBehaviours or Unity's components. That's because it's scripts are self-adapted to show more info in the Inspector
when running from the editor, but in the final build it looks almost like just piece of data (what they are meant to be). SLE components
are as simple to use as drag-n-drop Unity's default components, they are very customizable and as much self-explanatory as possible. Although some of them can contain Methods/Functions (in addition to data), these methods are just wrappers to communicate with it's systems
so they can be properly adjusted by them. None of component behaviors are contained in itself.
   Some of the components actions are lead by internal event triggers, that is, the component itself does nothing, but it signals its system
that it should do something with it.

![image](https://user-images.githubusercontent.com/61104009/136673278-1266b853-424f-4127-b222-1bb21fc40377.png) <br/>
<i>Health and Health bar components example. <br/>
Note: The Health Bar component can only be used in conjunction with a Health component. (The inverse does not apply) <br/>
Note2: The health bar object is auto-generated at runtime by the Health System.</i>

<b> - How to use - </b>

   First of all, you need to make some changes on your project to run the SLE System properly.
   
   First-things-first you need one object (can be any) to hold the <i>World</i> script. 
   This object act as a "Game Manager" and must have only one instance of it in the scene. It cannot be destroyed during the scene execution. If there's more than one <i>World</i> components in the scene, the first one will be defined as the scene instance and the subsequent gonna destroys your game object, be cautios.
For each scene you have, you'll gonna need it's own <i>World</i> instance. It's because some scenes may contain some data and/or systems that another scene shouldn't, each time the scene is loaded the <i>World</i> initializes itself and the Systems it should load. When the scene (or the <i>World</i> itself) is destroyed, the <i>World</i> starts 
the process of stopping it's systems, freeing it's used memories and let your game free to start another stage/state of your game execution.
   
   The <i>World</i> component is the one responsible to make the systems runs, without it, none of your systems may work at all.
   
   Once you have your scene's <i>World</i> instance in place, you must go to the Edit>Project Settings>Script Execution Order.
You gonna place the 'SLE.World' just on top of the default Unity's execution time. This way the <i>World</i> will run it's tasks just before ANY user defined scripts
and finish as soon as possible leaving the rest of the frame execution to the Engine pipeline, also, it makes possible to the user to iteract with any SLE component 
without needs to worry about references or anything else.

   ![image](https://user-images.githubusercontent.com/61104009/136671102-4afbdc4c-29cc-48cf-ab98-eb8583562fe1.png)
   
   And that's really it! Now you are ready-to-go with your games production :D !
   
<b><i> More details will be posted with time. </i></b>
