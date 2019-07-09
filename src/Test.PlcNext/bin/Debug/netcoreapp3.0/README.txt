------------
Prerequisite
------------

+ Only on linux systems: libunwind8 must be installed

---------------------
How to setup the CLIF
---------------------

+ When SDKs are not yet installed, install them with the following command:
  'plcncli install sdk -d "<path/to/destination/directory>" "<path/to/tar.gz/or/sh>"'
+ When SDKs are already installed, add each SDK to the settings with the 
  command: 
  'plcncli set setting --add SdkPaths "<path\to\SDK>"'
+ If a previous version of the CLIF was installed (19.0.X), before executing
  any command, execute the command 'migrate-old-cli'

----------------------------------
Example on how to create a library
----------------------------------

<Path to command> new project -n My.Company.Product
<Path to command> set target -n AXCF2152 --add -p My.Company.Product
<Path to command> generate all -p My.Company.Product
<Path to command> build -p My.Company.Product
<Path to command> deploy -p My.Company.Product

--------------------------------------
How to define a component/program port
--------------------------------------

+ Place a comment above the field of the port
+ The comment need to have the format '//#port'
+ Attributes can be defined with the comment '//#attributes(<attributes>)'
	+ Everything inside the bracket will be used as attributes for the port,
	  such as Input, separated by '|'
+ The port field need to be public

-------------
Miscellaneous
-------------

Q.	What is the '//#component(<Name of a component>)' comment above the class
definition of a program?

A.	It defines the component which manages the program. It can be changed to
change the managing component. If no such comment is found, the last namespace
will be used as the name for the managing component.

Q.	Can I change the prefix '#' for all markings such as '#port' and
'#component'?

A.	Yes. In the 'settings.xml' is an attribute called 'AttributePrefix'. This
value is used to define the prefix. Warning: This setting applies to all
projects. All attributes need to be changed accordingly.