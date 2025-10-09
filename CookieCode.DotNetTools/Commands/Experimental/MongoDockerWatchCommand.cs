using System.Collections.Generic;
using Spectre.Console.Cli;
using Terminal.Gui;

namespace CookieCode.DotNetTools.Commands.Experimental
{
	public class State
	{
		public required List<Container> Containers { get; set; }
		public Container? SelectedContainer { get; set; }
		public Database? SelectedDatabase { get; set; }
		public Collection? SelectedCollection { get; set; }
		public Document? SelectedDocument { get; set; }
	}

	public class Container
	{
		public required string Name { get; set; }
		public required string Port { get; set; }
		public required List<Database> Databases { get; set; }
	}

	public class Database
	{
		public required string Name { get; set; }
		public required Container Parent { get; set; }
		public required List<Collection> Collections { get; set; }
	}

	public class Collection
	{
		public required string Name { get; set; }
		public required Database Parent { get; set; }
		public required List<Document> Documents { get; set; }
	}

	public class Document
	{
		public required string Name { get; set; }
		public required string Text { get; set; }
	}

	internal class MongoDockerWatchCommand : Command<MongoDockerWatchCommand.Settings>
	{
		public class Settings : CommandSettings
		{
		}

		public override int Execute(CommandContext context, Settings settings)
		{
			Application.Init();

			// Define a custom color scheme
			Colors.Base = new ColorScheme
			{
				Normal = Application.Driver.MakeAttribute(Color.BrightGreen, Color.Black),
				Focus = Application.Driver.MakeAttribute(Color.BrightYellow, Color.Black),
				HotNormal = Application.Driver.MakeAttribute(Color.Cyan, Color.Black),
				HotFocus = Application.Driver.MakeAttribute(Color.Magenta, Color.Black)
			};

			// Create the main window
			var window = CreateWindow();

			Application.Top.Add(window);
			Application.Run();
			Application.Shutdown();
			return 0;
		}

		private View _containersSection;

		private Window CreateWindow(string title = "Two Column Layout", string rightText = "some sample text")
		{
			var window = new Window(title)
			{
				X = 0,
				Y = 0,
				Width = Dim.Fill(),
				Height = Dim.Fill()
			};

			// Create the left column with static positioning
			var leftColumn = new View
			{
				X = 0,
				Y = 0,
				Width = Dim.Percent(25),
				Height = Dim.Fill()
			};

			// Add static accordion sections to the left column
			var containersSection = CreateAccordionSection("Containers", 0, new Label("Container 1"), new Label("Container 2"));
			var databasesSection = CreateAccordionSection("Databases", 5, new Label("Database 1"), new Label("Database 2"));
			var collectionsSection = CreateAccordionSection("Collections", 10, new Label("Collection 1"), new Label("Collection 2"));
			var documentsSection = CreateAccordionSection("Documents", 15, new Label("Document 1"), new Label("Document 2"));

			// Add sections to the left column
			leftColumn.Add(containersSection, databasesSection, collectionsSection, documentsSection);

			// Create the right column with a non-editable multi-line TextView
			var rightColumn = new View
			{
				X = Pos.Right(leftColumn),
				Y = 0,
				Width = Dim.Fill(),
				Height = Dim.Fill()
			};

			var rightTextView = new TextView
			{
				Text = rightText,
				X = 1,
				Y = 1,
				Width = Dim.Fill() - 2,
				Height = Dim.Fill() - 2,
				ReadOnly = true
			};

			rightColumn.Add(rightTextView);

			// Add left and right columns to the main window
			window.Add(leftColumn);
			window.Add(rightColumn);

			return window;
		}

		// Helper method to create each accordion section without dependencies
		private View CreateAccordionSection(string title, int yPos, params View[] childViews)
		{
			// Simple container for accordion section
			var section = new View
			{
				X = 0,
				Y = yPos,
				Width = Dim.Fill(),
				Height = Dim.Sized(5) // Fixed height for each section
			};

			// Accordion header button
			var headerButton = new Button(title)
			{
				X = 0,
				Y = 0,
				Width = Dim.Fill(),
				Height = 1
			};

			// Add header button to section
			section.Add(headerButton);

			// Position child views and make them initially hidden
			int childY = 1;
			foreach (var child in childViews)
			{
				child.X = 1;
				child.Y = childY++;
				child.Visible = false;
				section.Add(child);
			}

			// Toggle visibility of child views when header button is clicked
			headerButton.Clicked += () =>
			{
				bool isVisible = childViews[0].Visible;
				foreach (var child in childViews)
					child.Visible = !isVisible;
			};

			return section;
		}
	}
}
