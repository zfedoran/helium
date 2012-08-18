Helium Animation Library
========================
The goal of this library is to allow programmers to easily animate their models in any way they desire. The XNA framework makes many parts of game development comfortable, however, the XNA team has decided to exclude an animation framework in their API. Furthermore, the XNA Model class makes it a pain to access vertex and bone data. The Helium library is faster and supports more features than the XNA skinned model [sample](http://create.msdn.com/en-US/education/catalog/sample/skinned_model). 

<a href="http://www.youtube.com/watch?v=JGUQkWK6s1E"><img src="https://github.com/zfedoran/helium/raw/master/screenshot.png"></a>

Check out the <a href="http://www.youtube.com/watch?v=JGUQkWK6s1E">demo video</a>

Please have a look at the primary [source code](https://github.com/zfedoran/helium/tree/master/helium/helium/animation) directory if you have any questions.

FAQ
===

**Q:** How do I use this library?

**A:** This library exposes animation content, it is up to you to decide how to use this content. An example is provided (See the [ExampleModelClass](https://github.com/zfedoran/Helium-Animation-Library/blob/master/helium/helium/examplemodel.cs)).

	
**Q:** What types of animations does the library support?

**A:** The library supports skinned and unskinned bone animations, and mesh hierarchy animations.
 

**Q:** Is the Helium Animation Library compatible with Mono and Monogame?

**A:** No, Monogame does not yet support 3D content.  
 
 
**Q:** What are the features of the library?


* Exposes animation content
* Works on fbx and x model formats
* Animation frame interpolating (play animations backwards, forwards, at half speed, etc...) 
* Based on the animation system found in [Ogre3d](http://www.ogre3d.org/)
