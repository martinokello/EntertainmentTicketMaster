#Installation Instructions

**Requirements**
This package requires Sass (Compass is optional).  
For Compass and Sass support in Visual Studio see: http://www.mindscapehq.com/products/web-workbench

##1. Configure your project

Foundation can be used either with Sass or Sass with compass. The Foundation Nuget package installs Foundation to the `~/sass` folder by default.

1a. Configure your Sass output folder
Open up the Web Workbench Settings for your project (select the project and use the right click context menu, or the Mindscape menu) 
you can set the Output Folder for your SCSS file and specify where the .css and/or .min.css will be generated.

1b. Create a Compass project
(Instructions using Web Workbench) http://www.mindscapehq.com/blog/index.php/2012/10/04/working-with-compass-web-workbench-to-create-button-sprites-within-visual-studio/
**If you are using Web Workbench, you must create a Compass project, otherwise Web Workbench will not save the compiled CSS to the correct location.**

Open the `/config.rb` and change the output directory from `stylesheets` to `Content`
`#Match MVC conventions`
`css_dir = "Content"`

If desired remove Compass default files `ie.scss, screen.scss, print.scss`

##2. Replace the Remove the Bootstrap theme

Remove the default Bootstrap theme:

From the package manager console run **`PM> Uninstall-Package Bootstrap`**

Sass will replace the default style `~/Content/Site.css` This **WILL BE OVERWRITTEN** each time Sass compiles

Open and save `~/sass/Site.scss` (SASS) to generate `~/Content/Site.css` (CSS)

##3. You are now ready to begin building your MVC project using Foundation.

####Related Nuget packages
Want to rapid prototype and wire frame directly from code using Html Helpers? 
Try the prototyping package on nuget. It works great with Foundation.
http://www.nuget.org/packages/Prototyping_MVC

Having trouble with media queries? Debug them with this simple CSS file.
http://nuget.org/packages/CSS_Media_Query_Debugger

####Documentation
Docs http://foundation.zurb.com/docs/  
Demo http://edcharbeneau.github.com/FoundationSinglePageRWD/

Resources: http://www.responsiveMVC.net/

Follow us:  
Ed Charbeneau http://twitter.com/#!/edcharbeneau  
Foundation Zurb http://twitter.com/#!/foundationzurb

#####Change Log:
Version 2.0.530
    - Updated Foundation to 5.3.0 (Foundation for Sites) http://zurb.com/article/1316/foundation-strike-5-3-strike-for-sites

Version 1.4.523
    - Updated Foundation to 5.2.3
    - 
Version 1.4.522
    - Updated Foundation to 5.2.2

Version 1.4.521
    - Updated Foundation to 5.2.1

Version 1.4.511
    - Updated Foundation to 5.1.1
    - Streamlinied the install process. Foundation will now overwrite the necessary files to minimize setup.

Version 1.0.502
	- Initial NuGet Release

Note: version scheme `<major>.<minor>.<foundation version>`
foundation version represents the foundation version less the "." for example 4.1.4 would be #.#.414

Foundation Framework Support:
http://foundation.zurb.com/docs