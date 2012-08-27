using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

// The class ItemWrapper wraps around an item of unknown type, 
// with name and dependencies metadata that may reference
// other Item objects by name

public class ItemWrapper
{
	// optional parameters in constructor allows four categories of items:
	// 		unnamed with no dependencies
	// 		unnamed with dependencies
	// 		named with no dependencies
	// 		named with dependencies	

	public ItemWrapper(object item, string name = "", string dependencyList = "")
	{
		this.name = name;
		this.item = item;
		incomingDependencies = new Stack<ItemWrapper>();
		outgoingDependencies = new List<ItemWrapper>();

		if (String.IsNullOrEmpty(dependencyList)) 
		{
			this.dependencies = null;
		} 
		else
		{
			this.dependencies = dependencyList.Split(',');
		}
	}

	public string[] GetDependencies() 
	{
		return this.dependencies;
	}

	public object GetItem() 
	{
		return this.item;
	}

	public string GetName() 
	{
		return this.name;
	}

	public void AddIncomingDependency(ItemWrapper item) 
	{
		incomingDependencies.Push(item);
	}

	public ItemWrapper RemoveIncomingDependency() 
	{
		if (incomingDependencies.Count > 0)
			return incomingDependencies.Pop();
		else
			return null;
	}

	public bool HasNoIncomingDependency() 
	{
		if (incomingDependencies.Count > 0)
			return false;
		else
			return true;
	}
	
	public void AddOutgoingDependency(ItemWrapper item) 
	{
		outgoingDependencies.Add(item);
	}

	public void RemoveOutgoingDependency(ItemWrapper item) 
	{
		if (outgoingDependencies.Contains(item))
			outgoingDependencies.Remove(item);
	}

	public bool HasNoOutgoingDependency() 
	{
		if (outgoingDependencies.Count > 0)
			return false;
		else
			return true;
	}

	// compare whether two ItemWrapper classes are same by comparing
	// their name and their array of unique dependencies

	public bool Equals(ItemWrapper item)
    {		
		if (!item.GetName().Equals(this.name))
			return false;
		
		string[] otherDependencies = item.GetDependencies();
		if (otherDependencies == null && dependencies == null)
			return true;
		else if (dependencies == null || otherDependencies == null)
			return false;
		
		otherDependencies = otherDependencies.Distinct().ToArray();
		dependencies = dependencies.Distinct().ToArray();
		Array.Sort(otherDependencies);
		Array.Sort(dependencies);

		for (int i = 0; i < dependencies.Length; i++) 
		{
			if (!dependencies[i].Equals(otherDependencies[i]))
				return false;
		}

       return true;
    }

	private string name;  // name of this item
	private object item;  // item of unknown type
	private string[] dependencies;	// list of item names it depends on
	private Stack<ItemWrapper> incomingDependencies;  // track elements to traverse
	private List<ItemWrapper> outgoingDependencies; // track elements to traverse 
}

// A dummy struct ADT for testing purposes, the item itself

struct Item
{
    private string val;
    public string Data
    {
        get 
        {
            return val;
        }
        set 
        {
            val = value;
        }
    }

	public Item(string data) 
	{
		val = data;
	}

    public void PrintData()
    {
        Console.WriteLine("\t" + val);
    }
}

public class ItemSort
{
	public static void Main ()
	{
		Console.WriteLine("************************************************************");
		ExecuteTestCase(1,TestCase1(),"Normal execution with all 4 categories of items");
		ExecuteTestCase(2,TestCase2(),"Error - Items with cyclic references");
		ExecuteTestCase(3,TestCase3(),"Duplicate items with no dependencies (allowable)");
		ExecuteTestCase(4,TestCase4(),"Duplicate items with dependencies listed in same order (allowable)");
		ExecuteTestCase(5,TestCase5(),"Error - Duplicate names with different set of dependencies");
		ExecuteTestCase(6,TestCase6(),"All items have no dependencies");
		ExecuteTestCase(7,TestCase7(),"Maximum number of dependencies, without cyclic references");
		ExecuteTestCase(8,TestCase8(),"Error - Dependency on an non-existent item");
		ExecuteTestCase(9,TestCase9(),"Same dependency listed twice in same item (allowable)");
		ExecuteTestCase(10,TestCase10(),"Empty set of items");
		ExecuteTestCase(11,TestCase11(),"Duplicate items with same dependencies listed in different order (allowable)");

		Console.WriteLine("All testcases completed successfully.");
	}

	// this function executes testcase and prints out the data in the items
	// of the sorted list

	private static void ExecuteTestCase(int num, List<ItemWrapper> testcase, string msg) 
	{
		Console.WriteLine("Testcase " + num + ": " + msg + "\n");

		try {		
			List<ItemWrapper> sortedList = getSortedList(testcase);

			foreach (ItemWrapper item in sortedList)
			{
				((Item) item.GetItem()).PrintData();
			}

			if (ValidateList(sortedList))
				Console.WriteLine("\n****************************************************  Passed\n");
			else
				Console.WriteLine("\n****************************************************  Failed\n");
		}
		catch (Exception e)
		{
			Console.WriteLine(e.Message);
			if (!e.Message.Contains("Error:"))
				Console.WriteLine("\n****************************************************  Failed\n");
			else
				Console.WriteLine("\n****************************************************  Passed\n");
		}

	}

	// this function sorts the list of items and return sorted list

	private static List<ItemWrapper> getSortedList(List<ItemWrapper> items)  
    {
		Queue<ItemWrapper> itemsToTraverse = new Queue<ItemWrapper>();		
		List<ItemWrapper> sortedItems = new List<ItemWrapper>();
		Dictionary<string, ItemWrapper> indexes = new Dictionary<string, ItemWrapper>();  

		// store the locations of items in list by name in dictionary,
		// allows easier access later when adding edges (dependencies)

        for (int i = 0; i < items.Count; i++)  
        {  
			if (!String.IsNullOrEmpty(items[i].GetName()))
			{
				// check for duplicates, they are allowed only if both name and unique 
				// dependencies are same

				if (indexes.ContainsKey(items[i].GetName()) && 
					!indexes[items[i].GetName()].Equals(items[i]))
					throw new Exception("\tError: Duplicate item with conflicting dependencies");
				else
            		indexes[items[i].GetName()] = items[i];
			}
        }  

        // add edges, both incoming (other items depending on itself) and
		// outgoing (items it depends on)  

        for (int i = 0; i < items.Count; i++)  
        {  
			string[] dependsOn = items[i].GetDependencies();
            if (dependsOn != null)  
            {  
				foreach (string name in dependsOn)
                {  
					if (indexes.ContainsKey(name)) 
					{
		                indexes[name].AddIncomingDependency(items[i]);
						items[i].AddOutgoingDependency(indexes[name]);
					}
					else
					{
						throw new Exception("\tError: An item has dependency on an non-existent item");
					}
                }  
            }  
			else
			{
				// add nodes with no dependencies to the queue for traversal

				itemsToTraverse.Enqueue(items[i]);
			}
        }  
  
		// continue to traverse through queue until all items are processed

		while (itemsToTraverse.Count != 0)
		{
			// add current item to the sorted list

			ItemWrapper processingItem = itemsToTraverse.Dequeue();
			sortedItems.Add(processingItem);

			// process each item that depended on this current item, and remove
			// outgoing link from that current item

			ItemWrapper nextItem = processingItem.RemoveIncomingDependency();

			while (nextItem != null)
			{
				nextItem.RemoveOutgoingDependency(processingItem);
				
				// add the item to traversal queue if it has no more dependencies

				if (nextItem.HasNoOutgoingDependency())
					itemsToTraverse.Enqueue(nextItem);					
					
				nextItem = processingItem.RemoveIncomingDependency();
			}
		}
		
		// all items are now sorted, but if there were cyclic references
		// in the items, not all items will be in the final sorted list
		// since at some point there will be no more items without dependencies 
		// to traverse

		if (sortedItems.Count != items.Count) 
		{        
            throw new Exception("\tError: Cyclic references detected in list of items");  
		} 
		else 
		{
			return sortedItems;
		}
	}

	// function for validating the list of items, i.e. items with dependencies
	// should come after the items it depends on

	private static bool ValidateList(List<ItemWrapper> items) 
	{		
		List<string> names = new List<string>(); 

		foreach (ItemWrapper item in items) 
		{
			if (item.GetDependencies() != null)
			{
				foreach (string name in item.GetDependencies())
				{
					if (!names.Contains(name))
						return false;
				}
			}
			if (!String.IsNullOrEmpty(item.GetName()))
				names.Add(item.GetName());
		}
		
		return true;
	}

	// Testcase 1: normal execution with items in all 4 categories
	private static List<ItemWrapper> TestCase1()
	{
		List<ItemWrapper> items = new List<ItemWrapper>();
		
		items.Add(new ItemWrapper(new Item("s100 dependsOn s50,s60"), "s100", "s50,s60"));
		items.Add(new ItemWrapper(new Item("s90 dependsOn s50,s80"), "s90", "s50,s80"));
		items.Add(new ItemWrapper(new Item("s80 dependsOn s50"), "s80", "s50"));
		items.Add(new ItemWrapper(new Item("s70"), "s70"));
		items.Add(new ItemWrapper(new Item("s60"), "s60"));
		items.Add(new ItemWrapper(new Item("s50"), "s50"));
		items.Add(new ItemWrapper(new Item("unnamed1")));
		items.Add(new ItemWrapper(new Item("unnamed2")));
		items.Add(new ItemWrapper(new Item("unnamed3 dependsOn s70"), dependencyList: "s70"));
		items.Add(new ItemWrapper(new Item("unnamed4 dependsOn s80"), dependencyList: "s80"));

		return items;
	}

	// Testcase 2: cyclic references, should throw exception
	private static List<ItemWrapper> TestCase2()
	{
		List<ItemWrapper> items = new List<ItemWrapper>();
		
		items.Add(new ItemWrapper(new Item("s100 dependsOn s50,s60"), "s100", "s50,s60"));
		items.Add(new ItemWrapper(new Item("s90 dependsOn s80"), "s90", "s80"));
		items.Add(new ItemWrapper(new Item("s80 dependsOn s50"), "s80", "s50"));
		items.Add(new ItemWrapper(new Item("s70"), "s70"));
		items.Add(new ItemWrapper(new Item("s60"), "s60"));
		items.Add(new ItemWrapper(new Item("s50 dependsOn s90"), "s50", "s90"));

		return items;
	}

	// Testcase 3: duplicate items with no dependencies, this is allowable as no logic is
	//				broken, the sorted list will still be in correct sequence
	private static List<ItemWrapper> TestCase3()
	{
		List<ItemWrapper> items = new List<ItemWrapper>();
		
		items.Add(new ItemWrapper(new Item("s100 dependsOn s50,s60"), "s100", "s50,s60"));
		items.Add(new ItemWrapper(new Item("s90 dependsOn s80"), "s90", "s80"));
		items.Add(new ItemWrapper(new Item("s80 dependsOn s50"), "s80", "s50"));
		items.Add(new ItemWrapper(new Item("s60"), "s60"));
		items.Add(new ItemWrapper(new Item("s60"), "s60"));
		items.Add(new ItemWrapper(new Item("s50"), "s50"));

		return items;
	}

	// Testcase 4: duplicate items with dependencies, this is allowable as no logic is
	//				broken, the sorted list will still be in correct sequence
	private static List<ItemWrapper> TestCase4()
	{
		List<ItemWrapper> items = new List<ItemWrapper>();
		
		items.Add(new ItemWrapper(new Item("s100 dependsOn s50,s60"), "s100", "s50,s60"));
		items.Add(new ItemWrapper(new Item("s90 dependsOn s80"), "s90", "s80"));
		items.Add(new ItemWrapper(new Item("s80 dependsOn s60"), "s80", "s60"));
		items.Add(new ItemWrapper(new Item("s60 dependsOn s50"), "s60", "s50"));
		items.Add(new ItemWrapper(new Item("s60 dependsOn s50"), "s60", "s50"));
		items.Add(new ItemWrapper(new Item("s50"), "s50"));

		return items;
	}

	// Testcase 5: duplicate names with different set of dependencies, should throw exception
	//				since it defies logic
	private static List<ItemWrapper> TestCase5()
	{
		List<ItemWrapper> items = new List<ItemWrapper>();
		
		items.Add(new ItemWrapper(new Item("s100 dependsOn s50,s60"), "s100", "s50,s60"));
		items.Add(new ItemWrapper(new Item("s90 dependsOn s80"), "s90", "s80"));
		items.Add(new ItemWrapper(new Item("s80 dependsOn s50"), "s80", "s50"));
		items.Add(new ItemWrapper(new Item("s60 dependsOn s50"), "s60", "s50"));
		items.Add(new ItemWrapper(new Item("s60 dependsOn s40"), "s60", "s40"));
		items.Add(new ItemWrapper(new Item("s50"), "s50"));
		items.Add(new ItemWrapper(new Item("s40"), "s40"));

		return items;
	}

	// Testcase 6: all items have no dependencies
	private static List<ItemWrapper> TestCase6()
	{
		List<ItemWrapper> items = new List<ItemWrapper>();
		
		items.Add(new ItemWrapper(new Item("s60"), "s60"));
		items.Add(new ItemWrapper(new Item("s50"), "s50"));
		items.Add(new ItemWrapper(new Item("s40"), "s40"));
		items.Add(new ItemWrapper(new Item("unnamed1")));
		items.Add(new ItemWrapper(new Item("s10"), "s10"));
		items.Add(new ItemWrapper(new Item("unnamed2")));
		items.Add(new ItemWrapper(new Item("s20"), "s20"));
		items.Add(new ItemWrapper(new Item("s30"), "s30"));

		return items;
	}

	// Testcase 7: Maximum number of dependencies, without cyclic references
	private static List<ItemWrapper> TestCase7()
	{
		List<ItemWrapper> items = new List<ItemWrapper>();

		items.Add(new ItemWrapper(new Item("s100 dependsOn s50,s60,s70,s80,s90"), "s100", "s50,s60,s70,s80,s90"));
		items.Add(new ItemWrapper(new Item("s90 dependsOn s50,s60,s70,s80"), "s90", "s50,s60,s70,s80"));
		items.Add(new ItemWrapper(new Item("s80 dependsOn s50,s60,s70"), "s80", "s50,s60,s70"));
		items.Add(new ItemWrapper(new Item("s70 dependsOn s50,s60"), "s70", "s50,s60"));
		items.Add(new ItemWrapper(new Item("s60 dependsOn s50"), "s60", "s50"));
		items.Add(new ItemWrapper(new Item("s50"), "s50"));

		return items;
	}

	// Testcase 8: dependency on an non-existent item - should throw exception
	private static List<ItemWrapper> TestCase8()
	{
		List<ItemWrapper> items = new List<ItemWrapper>();
		
		items.Add(new ItemWrapper(new Item("s100 dependsOn s50,s60"), "s100", "s50,s60"));
		items.Add(new ItemWrapper(new Item("s90 dependsOn s80"), "s90", "s80"));
		items.Add(new ItemWrapper(new Item("s80 dependsOn s50"), "s80", "s50"));
		items.Add(new ItemWrapper(new Item("s60 dependsOn s40"), "s60", "s40"));
		items.Add(new ItemWrapper(new Item("s50"), "s50"));

		return items;
	}

	// Testcase 9: same dependency listed twice in same item, should ignore dependency
	// 				if already seen and edge added, and this is allowable as no logic is broken
	private static List<ItemWrapper> TestCase9()
	{
		List<ItemWrapper> items = new List<ItemWrapper>();
		
		items.Add(new ItemWrapper(new Item("s100 dependsOn s50,s90"), "s100", "s50,s90"));
		items.Add(new ItemWrapper(new Item("s90 dependsOn s80"), "s90", "s80"));
		items.Add(new ItemWrapper(new Item("s80 dependsOn s50,s50,s50"), "s80", "s50,s50,s50"));
		items.Add(new ItemWrapper(new Item("s50"), "s50"));

		return items;
	}

	// Testcase 10: empty set of items, nothing needs to be done
	private static List<ItemWrapper> TestCase10()
	{
		List<ItemWrapper> items = new List<ItemWrapper>();
		
		return items;
	}

	// Testcase 11: duplicate names with same set of dependencies in different order,
	//				this is allowable as no logic is broken
	private static List<ItemWrapper> TestCase11()
	{
		List<ItemWrapper> items = new List<ItemWrapper>();
		
		items.Add(new ItemWrapper(new Item("s100 dependsOn s50,s60"), "s100", "s50,s60"));
		items.Add(new ItemWrapper(new Item("s90 dependsOn s80"), "s90", "s80"));
		items.Add(new ItemWrapper(new Item("s80 dependsOn s50"), "s80", "s50"));
		items.Add(new ItemWrapper(new Item("s60 dependsOn s50,s40,s40"), "s60", "s50,s40,s40"));
		items.Add(new ItemWrapper(new Item("s60 dependsOn s40,s50"), "s60", "s40,s50"));
		items.Add(new ItemWrapper(new Item("s50"), "s50"));
		items.Add(new ItemWrapper(new Item("s40"), "s40"));

		return items;
	}

}


