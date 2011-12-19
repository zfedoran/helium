Helium Animation Library
========================
The goal of this library is to allow programmers to easily animate their models in any way they desire. The XNA framework makes many parts of game development comfortable, however, the XNA team has decided to exclude an animation framework in their API. Furthermore, the XNA Model class makes it a pain to access vertex and bone data. 

FAQ
===

How do I use this library?

**A:** This library exposes animation content, it is up to you to decide how to use this content. An example is provided (See the ExampleModelClass class).

	
What types of animations does the library support?

**A:** The library supports skinned and unskinned bone animations, and mesh hierarchy animations.
 
 
What are the features of the library?


* Exposes animation content
* Skinned and unskinned animation
* Models with multiple meshes
* Works on fbx and x model formats
* Animation frame interpolating (play animations backwards, forwards, at half speed, etc...) 
* Allows manual setting of bone transforms
* Framework for custom content pipleine animation and model processing
* Allows use of custom .fx effects
* Based on the animation system found in OGRE3d
