using GBX.NET.Engines.Game;
using GBX.NET;
using GBX.NET.Engines.GameData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;
using GBX.NET.Engines.Plug;
using System.Numerics;
using System.Globalization;

namespace Triangle3DAnimation
{
	internal class Program
	{
		public string programTitle = @"
  ________  _________               ______                      ____         
 /_  __/  |/  /  _/ /____  ____ ___/_  __/________ _____  _____/ __/__  _____
  / / / /|_/ // // __/ _ \/ __ `__ \/ / / ___/ __ `/ __ \/ ___/ /_/ _ \/ ___/
 / / / /  / // // /_/  __/ / / / / / / / /  / /_/ / / / (__  ) __/  __/ /    
/_/ /_/  /_/___/\__/\___/_/ /_/ /_/_/ /_/   \__,_/_/ /_/____/_/  \___/_/     

";

		public bool isDisplayingLogs = false;
		public bool isUsingAssisted = false;
		public enum SearchMethod
		{
			Simple,
			Complex
		}

		public static void Main(string[] args){
			Program p = new Program();
			bool isDisplayingTime = false;
			bool isDisplayingHowTo = false;
			bool isDisplayingHelp = false;
			string inputMap = "";
			string outputMap = "";
			Vec3 offset = default;
			Console.Clear();
			for (int i = 0; i < args.Count(); i++)
			{
				if(args[i] == "--display-logs") p.isDisplayingLogs = true;
				if(args[i] == "--time") isDisplayingTime = true;
				if(args[i] == "--howto") isDisplayingHowTo = true;
				if(args[i] == "--assisted") p.isUsingAssisted = true;
				if(args[i] == "--help" || args[i] == "-H" || args[i] == "help") isDisplayingHelp = true;
				if(args[i].Contains("--input-map")){
					string[] arg = args[i].Split("=");
					if(arg.Count() > 1){
						inputMap = arg[1];
					}
				}
				if(args[i].Contains("--output-map")){
					string[] arg = args[i].Split("=");
					if(arg.Count() > 1){
						outputMap = arg[1];
					}
				}
				if(args[i].Contains("--offset")){
					string[] arg = args[i].Split("=");
					if(arg.Count() > 1){
						string[] pos = arg[1].Split(",");
						if(pos.Count() == 3){
							offset = new Vec3(
								float.Parse(pos[0], CultureInfo.InvariantCulture.NumberFormat),
								float.Parse(pos[1], CultureInfo.InvariantCulture.NumberFormat),
								float.Parse(pos[2], CultureInfo.InvariantCulture.NumberFormat)
							);
						}
					}
				}
			}

			if(isDisplayingHelp){
				p.DisplayHelp();
			}else if(isDisplayingHowTo){
				p.DisplayHowTo();
			}else{
				var watch = new System.Diagnostics.Stopwatch();
				watch.Start();

				// DEV
				inputMap = "Concrete";
				outputMap = "Concrete_TM2020";
				offset = new Vec3(0f, 70.2f, 0f);

				if(p.isUsingAssisted){
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine(p.programTitle);
					Console.ResetColor();

					Console.Write("\u25BA Input map name (without \".Map.Gbx\"): ");
					inputMap = Console.ReadLine() + "";
					Console.Write("\u25BA Output map name (without \".Map.Gbx\"): ");
					outputMap = Console.ReadLine() ?? "";
					Console.WriteLine("\u25BA Items offset (optional, press enter for default): ");
					Console.Write("	\u25BA Offset X: ");
					string offsetX = Console.ReadLine() + "";
					offsetX = String.IsNullOrEmpty(offsetX) ? "0" : offsetX;
					Console.Write("	\u25BA Offset Y: ");
					string offsetY = Console.ReadLine() + "";
					offsetY = String.IsNullOrEmpty(offsetY) ? "0" : offsetY;
					Console.Write("	\u25BA Offset Z: ");
					string offsetZ = Console.ReadLine() + "";
					offsetZ = String.IsNullOrEmpty(offsetZ) ? "0" : offsetZ;

					Console.WriteLine();

					// Console.WriteLine($"{float.Parse(offsetX, CultureInfo.InvariantCulture.NumberFormat)}, {float.Parse(offsetY, CultureInfo.InvariantCulture.NumberFormat)}, {float.Parse(offsetZ, CultureInfo.InvariantCulture.NumberFormat)}");
					offset = new Vec3(
						float.Parse(offsetX, CultureInfo.InvariantCulture.NumberFormat),
						float.Parse(offsetY, CultureInfo.InvariantCulture.NumberFormat),
						float.Parse(offsetZ, CultureInfo.InvariantCulture.NumberFormat)
					);
				}
				if(!String.IsNullOrEmpty(inputMap) && inputMap != "" && !String.IsNullOrEmpty(outputMap) && outputMap != ""){
					p.TM2_2_TM2020(inputMap, outputMap, SearchMethod.Complex, offset);
				}else{
					Console.ForegroundColor = ConsoleColor.Red; Console.Write("X"); Console.ResetColor(); Console.WriteLine($" Input map or Output map parameter not set.");
				}

				watch.Stop();
				if(isDisplayingTime) Console.WriteLine(string.Format("Time elapsed: {0:00}:{1:00}:{2:000}", watch.Elapsed.Minutes, watch.Elapsed.Seconds, watch.Elapsed.Milliseconds));
			}
		}

		public void DisplayHelp(){
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine(this.programTitle);
			Console.ResetColor();

			Console.WriteLine("Usage:");
			Console.WriteLine("	TMItemTransfer.exe [[--assisted] [--display-logs] [--time] [--howto] {--input-map --output-map} [--offset]]");
			Console.WriteLine();
			Console.WriteLine("Options:");
			Console.ForegroundColor = ConsoleColor.DarkYellow; Console.Write("	--help"); Console.ResetColor(); Console.WriteLine("			Display this help");
			Console.ForegroundColor = ConsoleColor.DarkYellow; Console.Write("	--assisted"); Console.ResetColor(); Console.WriteLine("		Launch the program with a step by step process");
			Console.ForegroundColor = ConsoleColor.DarkYellow; Console.Write("	--input-map={map_name}"); Console.ResetColor(); Console.WriteLine("	The input map (without \".Map.Gbx\")");
			Console.ForegroundColor = ConsoleColor.DarkYellow; Console.Write("	--output-map={map_name}"); Console.ResetColor(); Console.WriteLine("	The output map (without \".Map.Gbx\")");
			Console.ForegroundColor = ConsoleColor.DarkYellow; Console.Write("	--offset=X,Y,Z"); Console.ResetColor(); Console.WriteLine("		The offset to add to all the items (floats)");
			Console.ForegroundColor = ConsoleColor.DarkYellow; Console.Write("	--howto"); Console.ResetColor(); Console.WriteLine("			Displays the How-To");
			Console.ForegroundColor = ConsoleColor.DarkYellow; Console.Write("	--display-logs"); Console.ResetColor(); Console.WriteLine("		Prints all the logs");
			Console.ForegroundColor = ConsoleColor.DarkYellow; Console.Write("	--time"); Console.ResetColor(); Console.WriteLine("			Display elapsed time at the end of the transfer");

		}

		public void DisplayHowTo(){
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine(this.programTitle);
			Console.ResetColor();

			Console.WriteLine("1. All maps must be placed into the \"Maps/\" folder next to this exe file.");
			Console.WriteLine("2. All items must be placed into the \"Items/\" folder next to this exe file.");
			Console.WriteLine("3. The item paths and items names must be the same as the original map.");
			Console.WriteLine("4. The items must be compatible with the Trackmania the map is coming from.");
			Console.WriteLine("5. You must provide a TM2020 map that will receive all the items.");
			Console.WriteLine("6. You must set 2 arguments: --input-map={map_name} and --output-map={map_name}.");
			Console.WriteLine("7. {map_name} is the file name of the map, without \".Map.Gbx\" at the end. For example, the map file \"Map_File1.Map.Gbx\" must be set at \"--input-map=Map_File1\" (no space is allowed)");
			Console.WriteLine("8. A copy of the output map will be created by adding \"_Modified\" at the end of the file name.");
		}

		public void TM2_2_TM2020(string inputMapName, string outputMapName, SearchMethod searchMethod, Vec3 offset = default(Vec3)){
			if(!this.isUsingAssisted){
				Console.Clear();
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(this.programTitle);
				Console.ResetColor();
			}

			int loadingBarWidth = 50;

			if(!System.IO.File.Exists($"./Maps/{inputMapName}.Map.Gbx")){
				Console.ForegroundColor = ConsoleColor.Red; Console.Write("X"); Console.ResetColor(); Console.WriteLine($" Map {inputMapName}.Map.Gbx not found.");
				return;
			}
			if(!System.IO.File.Exists($"./Maps/{outputMapName}.Map.Gbx")){
				Console.ForegroundColor = ConsoleColor.Red; Console.Write("X"); Console.ResetColor(); Console.WriteLine($" Map {outputMapName}.Map.Gbx not found.");
				return;
			}
			var inputMap = GameBox.ParseNode<CGameCtnChallenge>($"./Maps/{inputMapName}.Map.Gbx");
			var outputMap = GameBox.ParseNode<CGameCtnChallenge>($"./Maps/{outputMapName}.Map.Gbx");
			
			string location = Environment.CurrentDirectory.Replace(@"\", "/");

			if(inputMap.AnchoredObjects is null){
				Console.ForegroundColor = ConsoleColor.Red; Console.Write("X"); Console.ResetColor(); Console.WriteLine($" Map {inputMapName}.Map.Gbx has no anchored object.");
				return;
			}
			IList<CGameCtnAnchoredObject> anchoredObjects = inputMap.AnchoredObjects;
			List<string> uniqueItems = new List<string>();
			Dictionary<string, CGameItemModel> itemInstance = new Dictionary<string, CGameItemModel>();
			List<string> availableItems = new List<string>();

			int counter = 0;
			string strListingAnchoredObjects = "\u25BA Listing anchored object(s) ";
			string strListingItemFiles = "\u25BA Listing item file(s) ";
			string strInstantingItems = "\u25BA Instantiating item(s) ";
			string strPlacingItems = "\u25BA Placing item(s) ";
			string strEmbeddingItems = "\u25BA Embedding item(s) ";


			if(this.isDisplayingLogs){
				Console.Write(strListingAnchoredObjects);
			}else{
				Console.WriteLine(strListingAnchoredObjects);
			}

			foreach (var anchoredObject in anchoredObjects) {
				if(!uniqueItems.Contains(anchoredObject.ItemModel.Id.Replace(".gbx", ".Gbx"))){
					uniqueItems.Add("/Items/"+anchoredObject.ItemModel.Id.Replace(".gbx", ".Gbx"));
				}
				counter++;
				if(this.isDisplayingLogs){
					switch (counter % 4){
						case 0: Console.Write("/"); break;
						case 1: Console.Write("-"); break;
						case 2: Console.Write("\\"); break;
						case 3: Console.Write("|"); break;
					}
					Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
				}else{
					float percentage = counter*100/anchoredObjects.Count();
					int barWidth = (int)Math.Round((double)percentage / 100 * loadingBarWidth);
					string loadingBar = "[" + new string('#', barWidth) + new string(' ', loadingBarWidth - barWidth) + "]";
					Console.Write($"\r{loadingBar} {percentage}%");
				}
			}
			if(this.isDisplayingLogs){
				Console.SetCursorPosition(Console.CursorLeft - strListingAnchoredObjects.Length, Console.CursorTop);
				Console.Write(strListingAnchoredObjects); Console.ForegroundColor = ConsoleColor.Green; Console.WriteLine("\u2713"); Console.ResetColor();
			}else{
				Console.WriteLine();
				Console.ForegroundColor = ConsoleColor.Green; Console.Write("\u2713"); Console.ResetColor(); Console.WriteLine($" Anchored item(s) listed");
				Console.WriteLine();
			}

			counter = 0;
			Console.Write(strListingItemFiles);
			foreach (string file in System.IO.Directory.GetFiles($"{location}/Items/", "*.Item.*", System.IO.SearchOption.AllDirectories)) {
				if(!availableItems.Contains(file.Replace(location, ""))){
					availableItems.Add(file.Replace(location, ""));
				}
				counter++;
				if(this.isDisplayingLogs){
					switch (counter % 4){
						case 0: Console.Write("/"); break;
						case 1: Console.Write("-"); break;
						case 2: Console.Write("\\"); break;
						case 3: Console.Write("|"); break;
					}
					Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
				}
			}

			if(this.isDisplayingLogs){
				Console.SetCursorPosition(Console.CursorLeft - strListingItemFiles.Length, Console.CursorTop);
				Console.Write(strListingItemFiles); Console.ForegroundColor = ConsoleColor.Green; Console.WriteLine("\u2713"); Console.ResetColor();
			}else{
				Console.WriteLine();
				Console.ForegroundColor = ConsoleColor.Green; Console.Write("\u2713"); Console.ResetColor(); Console.WriteLine($" Item file(s) listed");
			}
			Console.WriteLine();

			counter = 0;
			if(this.isDisplayingLogs){
				Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine("---------- INSTANTIATING ITEM(S) ----------"); Console.ResetColor();
			}else{
				Console.WriteLine(strInstantingItems);
			}
			foreach (var item in availableItems) {
				counter++;
				if(uniqueItems.Contains(item)){
					string itemName = item.Replace("/Items/", "").Replace("/", "_").Replace("\\", "_");
					itemInstance.Add(itemName, GameBox.ParseNode<CGameItemModel>($"{location}/"+item));
					if(this.isDisplayingLogs){
						Console.ForegroundColor = ConsoleColor.Green; Console.Write("\u2713"); Console.ResetColor(); Console.WriteLine($" Instancied item {itemName}");
					}
				}

				if(!this.isDisplayingLogs){
					float percentage = counter*100/availableItems.Count();
					int barWidth = (int)Math.Round((double)percentage / 100 * loadingBarWidth);
					string loadingBar = "[" + new string('#', barWidth) + new string(' ', loadingBarWidth - barWidth) + "]";
					Console.Write($"\r{loadingBar} {percentage}%");
				}
			}
			if(!this.isDisplayingLogs){
				Console.WriteLine();
				Console.ForegroundColor = ConsoleColor.Green; Console.Write("\u2713"); Console.ResetColor(); Console.WriteLine($" Item(s) instancied");
			}
			Console.WriteLine();

			if(this.isDisplayingLogs){
				Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine("---------- PLACING ITEM(S) ----------"); Console.ResetColor();
			}else{
				Console.WriteLine(strPlacingItems);
			}
			for (int i = 0; i < anchoredObjects.Count(); i++) {
				if(searchMethod == SearchMethod.Complex){
					CGameCtnAnchoredObject obj = anchoredObjects[i];
					string objName = obj.ItemModel.Id.Replace(".gbx", ".Gbx");
					if(availableItems.Contains("/Items/"+objName)) {
						if(itemInstance[objName.Replace("/", "_").Replace("\\", "_")].Ident.Author is not null) {
							outputMap.PlaceAnchoredObject(new Ident(objName, 26, itemInstance[objName.Replace("/", "_").Replace("\\", "_")].Ident.Author), (obj.AbsolutePositionInMap+offset), obj.PitchYawRoll, obj.PivotPosition);
							if(this.isDisplayingLogs){
								Console.ForegroundColor = ConsoleColor.Green; Console.Write("\u2713"); Console.ResetColor(); Console.WriteLine($" Item {objName} placed at {obj.AbsolutePositionInMap+offset}");
							}
						}
					}
					else {
						// Console.WriteLine($"Item not found at: {location}/Items/"+objName);
					}
				}

				if(!this.isDisplayingLogs){
					float percentage = i*100/anchoredObjects.Count();
					int barWidth = (int)Math.Round((double)percentage / 100 * loadingBarWidth);
					string loadingBar = "[" + new string('#', barWidth) + new string(' ', loadingBarWidth - barWidth) + "]";
					Console.Write($"\r{loadingBar} {percentage}%");
				}
			}
			if(!this.isDisplayingLogs){
				Console.WriteLine();
				Console.ForegroundColor = ConsoleColor.Green; Console.Write("\u2713"); Console.ResetColor(); Console.WriteLine($" Item(s) placed");
			}
			Console.WriteLine();

			if(this.isDisplayingLogs){
				Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine("---------- EMBEDDING ITEMS ----------"); Console.ResetColor();
			}else{
				Console.WriteLine(strEmbeddingItems);
			}

			if(inputMap.EmbeddedData is null){
				Console.ForegroundColor = ConsoleColor.Red; Console.Write("X"); Console.ResetColor(); Console.WriteLine($" Map {inputMapName}.Map.Gbx hqs no embedded data.");
				return;
			}
			Dictionary<string, byte[]> embeddedData = inputMap.EmbeddedData;
			for (int i = 0; i < embeddedData.Count(); i++) {
				if(searchMethod == SearchMethod.Complex) {
					KeyValuePair<string, byte[]> item = embeddedData.ElementAt(i);
					string objName = item.Key.Replace(".gbx", ".Gbx");

					if(System.IO.File.Exists($"{location}/{objName}")){
						// Console.WriteLine($"Embedded item {item.Key}");

						string itemPath = objName;
						List<string> itemPathExplode = itemPath.Split("/").ToList();
						itemPathExplode.RemoveAt(itemPathExplode.Count() - 1);
						itemPath = String.Join("/", itemPathExplode.ToArray());
						
						outputMap.ImportFileToEmbed($"{location}/{objName}", itemPath, true);
						if(this.isDisplayingLogs){
							Console.ForegroundColor = ConsoleColor.Green; Console.Write("\u2713"); Console.ResetColor(); Console.WriteLine($" Embedded {item.Key}");
						}
					}
					else {
						// Console.WriteLine(@"Item not found at: "+$"{location}/{objName}");
					}
				}

				if(!this.isDisplayingLogs){
					float percentage = i*100/embeddedData.Count();
					int barWidth = (int)Math.Ceiling((double)percentage / 100 * loadingBarWidth);
					string loadingBar = "[" + new string('#', barWidth) + new string(' ', loadingBarWidth - barWidth) + "]";
					Console.Write($"\r{loadingBar} {percentage}%");
				}
			}
			if(!this.isDisplayingLogs){
				Console.WriteLine("");
				Console.ForegroundColor = ConsoleColor.Green; Console.Write("\u2713"); Console.ResetColor(); Console.WriteLine($" Item(s) embedded");
			}
			Console.WriteLine("");
			
			outputMap.MapName = outputMap.MapName + " Modified";
			outputMap.Save($"Maps/{outputMapName}_Modified.Map.Gbx");
			// Console.WriteLine($"Saved map {ConsoleColor.Green}{outputMapName}_Modified.Map.Gbx{Console.ResetColor}");
			Console.ForegroundColor = ConsoleColor.Green;
			Console.Write("\u2713"); Console.ResetColor(); Console.Write($" Saved map ");
			Console.ForegroundColor = ConsoleColor.Green; Console.WriteLine($"{outputMapName}_Modified.Map.Gbx");
			Console.ResetColor();
		}
	}
}
