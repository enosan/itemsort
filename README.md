SUMMARY

This is a program contains a framework for sorting and verifying items with dependencies
using topological sorting.

METHODOLOGY

The main function of the program executes a number of testcases that tests all border
and corner cases or the program and framework.  For each testcase, a list of items is
constructed in a private testcase function within the main ItemSort class.  Each item
in the list is an ItemWrapper object that serves as a container for an object of unknown
type, packed with metadata.  Metadata includes a name, a string array (which contains 
the list of dependencies), and two collections (a stack and a list) for tracking edges
between each ItemWrapper objects when using topological sorting.

Each item can fall into one of four categories:
   - unnamed without dependencies
   - unnamed with dependencies
   - named without dependencies
   - named with dependencies

The list of ItemWrapper objects, which serves as the framework, is sorted using
topological sorting.  Namely, a function will take in the list, establish outgoing
and incoming links between the ItemWrapper objects, setup a queue with items that
has no dependencies, remove links between the items as they are processed, and add
new items with no dependencies to the queue until the queue is depleted entirely.

The result is a list of ItemWrapper objects in an order such that items with 
dependencies come later in the order than the item or items they depend on.

The sorted list will then go through a validator to check for the orderiness of
the list, and a pass or fail, along with the data contents of the objects itself,
are output onto the terminal.

Since the ItemWrapper class is a framework and a container for an object of unknown type,
a struct, DummyStruct (defined inside the program), acts as the object being contained.
This struct contains a simple string, a property to modify the string, and a function to
print the string.

CONSTRUCTING TESTCASES

If you would like to make your own testcase and construct your own list of items, you
will need to provide the following to the ItemWrapper constructor:

	-an object of any type you choose
	-optional: a name of the object
	-optional: a comma-seperated string of names it depends on

If the optional parameters are not provided, an empty string will be used as default.
A name with an empty string signifies an unnamed item.

Each testcase, constructed in a private function within ItemSort, can be executed in the main
function by adding the following line in the main function:

	ExecuteTestCase(1,TestCase1(),"Normal execution with all 4 categories of items");

	-first argument: id number of the testcase
	-second argument: List<ItemWrapper>, which should be returned by a private testcase function
	-third argument: description of the testcase

RUNNING THE PROGRAM

The program is tested using Ubuntu 11.10, with mono CSharp Compiler installed.  On Ubuntu,
the following commands were run to compile and execute:

	# gmcs itemsort.cs
	# mono itemsort.exe

