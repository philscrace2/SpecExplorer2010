# SpecExplorer2010
Promoting a legacy Model-based testing tool from Microsoft. I got it working in later versions of Visual Studio (see Readme.MD)

Spec Explorer lives! Getting Spec Explorer 2010 working in all versions of Visual Studio
One of the most powerful tools in a Software Test Engineers arsenal gets refreshed by the Engineer 1 making it work in VS2013, VS2015 and VS2017.

Spec Explorer 2010 a superb model-based testing tool from Microsoft Research gave hope to many by giving tester's a tool for modeling software, finding bugs early, and allowing us to finally get I'm control of large state spaces. Unfortunately support was suddenly dropped by MS after a few years of active development. The disappointment was felt by the masses (see https://visualstudio.uservoice.com/forums/121579-visual-studio-ide/suggestions/5595362-support-spec-explorer-in-visual-studio-2013) as users were left to upgrade to VS2013 and above for their development but leaving Spec Explorer 2010 to stand still in the only two supported versions VS2010 and VS2012.

In this article I will show you how to get Spec Explorer 2010 working in VS2013 and above so that you can once again enjoy testing in the 21st century.

With VS2010 or VS2012 installed

1. Run the SpecExplorer.msi. I chose a custom install and ensured the MSI installer had correctly detected that VS is installed, and then ran the installer to the end
2. Test Spec Explorer 2010 installed correctly by running steps Appendix A - steps 1 - 7
3. Install VS201X where X is 3 or 5 (i.e. VS2013 or VS2015)
4. Navigate to 'C:\Program Files (x86)\Microsoft Visual Studio 10.0\Common7\IDE\Extensions\Microsoft' (the old version location)
5. Copy the folder called 'Spec Explorer 2010'
6. Navigate to 'C:\Program Files (x86)\Microsoft Visual Studio 1X.0\Common7\IDE\Extensions\Microsoft' - note the 'X' in the path as this is the location of the new version of VS that you want to load the SE2010 plug in
7. Paste the 'Spec Explorer 2010' folder here
8. Reboot your machine
9. Open VS210X
10. Test Spec Explorer 2010 installed correctly by running steps Appendix A - steps 1 - 7

SE2010 is working

VS 2017 is a special case

After running steps 1-10 (but in step 3. install VS2017):

14. Run the VS2017 installer again and check the 'Extensions and extensibility' feature. Complete the install
15. Reboot your machine
16. Open VS2017

SE2010 is working in VS2017

Why does this work?

It turns out that Spec Explorer 2010 was written against an old Visual Studio SDK that utilised the Microsoft.VisualStudio.Shell.10.0.dll; a means of creating a plugin but this meant that later versions of this VS couldn't load the plugin. Well... actually later versions can, but how? Microsoft employs a binding redirect mechanism that tells Visual Studio that if it comes across an old version of a dll to load a different version. This setting is made in devenv.exe.config found here:

C:\Program Files (x86)\Microsoft Visual Studio\2017\Professional\Common7\IDE

...and the critical value is this:

name="Microsoft.VisualStudio.Shell.10.0"

bindingRedirect oldVersion="2.0.0.0-14.0.0.0" newVersion="15.0.0.0"

The guys at Microsoft have done a great job. Not only did they produce this great tool but they also produced a great Visual Studio extensibility framework that is backwards compatible!

Next time I'll explain how to get Spec Explorer 2010 UML extensions to work. In future posts I will also show you how to extend SE2010 and test the above procedure in VS2019 to see if it works.

As always some posts have helped me along the way:

https://msdn.microsoft.com/en-us/magazine/mt493251.aspx?f=255&MSPPError=-2147217396

Also, if you want to read an introduction to Model Based testing please read a blog post I wrote for Red Gate Ltd, now a strong partner of MS, written in 2013 but still very current!

https://www.red-gate.com/simple-talk/blogs/4712/

Appendix A.

1. Checked the 'Spec Explorer' menu appears in the menu bar
2. Clicked file menu and selected 'Open' ->  'Project/Solution...'
3. Navigate to 'Samples' folder found at 'c:\users\\My Documents\Spec Explorer 2010\samples
4. Navigate to the 'SMB2' folder and select and open the 'smb2.sln'
5. Build the Solution
6. From 'Spec Explorer' menu click 'Exploration Manager'
7. Right Mouse Click the machine called 'AllSync' and select Explore
8. After a few seconds a finite state machine will appear in the model viewer
