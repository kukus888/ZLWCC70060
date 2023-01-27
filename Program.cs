using SAPFEWSELib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace SKUassignment
{
    class Program
    {
        public static List<Model> Modely = new List<Model>();
        static void Main(string[] args)
        {
            //Load all the models
            Debug("Loading models...");
            LoadModels();
            Debug("Initializing SAP");

            //SAP init
            //Get the Windows Running Object Table
            SapROTWr.CSapROTWrapper sapROTWrapper = new SapROTWr.CSapROTWrapper();
            //Get the ROT Entry for the SAP Gui to connect to the COM
            object SapGuilRot = sapROTWrapper.GetROTEntry("SAPGUI");
            while(SapGuilRot == null)
            {
                //Sap isn't loaded.
                Debug("Please turn on SAP...");
                Thread.Sleep(1000);
                SapGuilRot = sapROTWrapper.GetROTEntry("SAPGUI");
            }
            //Get the reference to the Scripting Engine
            object engine = SapGuilRot.GetType().InvokeMember("GetScriptingEngine", System.Reflection.BindingFlags.InvokeMethod, null, SapGuilRot, null);
            //Get the reference to the running SAP Application Window
            GuiApplication GuiApp = (GuiApplication)engine;
            //Get connection
            GuiConnection connection = null;
            while (connection == null)
            {
                try
                {
                    //Get the reference to the first open connection
                    connection = (GuiConnection)GuiApp.Connections.ElementAt(0);
                }
                catch (System.Runtime.InteropServices.COMException ex)
                {
                    //Not connected yet
                    Debug("Please connect to CEP and Log In.");
                    Thread.Sleep(1000);
                }
            }
            GuiSession session = null;
            while(session == null)
            {
                try
                {
                    //get the first available session
                    session = (GuiSession)connection.Children.ElementAt(0);
                }
                catch (System.Runtime.InteropServices.COMException ex)
                {
                    //User not logged in, wait for login
                    Debug("Please Log In.");
                    Thread.Sleep(1000);
                }
            }
            //Get the reference to the main "Frame" in which to send virtual key commands
            GuiFrameWindow frame = (GuiFrameWindow)session.FindById("wnd[0]");


            List<KeyValuePair<string, Action>> MenuEntries = new List<KeyValuePair<string, Action>>();
            MenuEntries.Add(new KeyValuePair<string, Action>("Display models", () => { DisplayModels(); }));
            MenuEntries.Add(new KeyValuePair<string, Action>("Write to table", new Action(() =>
            {
                for (int i = 0; i <= Modely.Count - 1; i++)
                {
                    Debug($"Status: {i} / {Modely.Count}");
                    try
                    {
                        ApplyPackcodes(session, Modely[i]);
                    }
                    catch (Exception ex)
                    {
                        Error(ex.Message);
                        Error(ex.StackTrace);
                    }
                }
            })));
            MenuEntries.Add(new KeyValuePair<string, Action>("Check table", (() => { CheckPackcodes(session); })));
            Frontend.Menu("Menu", MenuEntries);


            /*GuiCollection comp = */
            //Debug($"{comp.Count} {comp.Item(0)} {comp.Type}");
            //Shell.ShowContextMenu();
            //Shell.SelectContextMenuItemByText("C493-P-GT-NXXCS0LH");*/
        }
        private static string GetValue(string OldVal)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Clear();
            Console.SetCursorPosition(1, 1);
            Console.WriteLine("Changing " + OldVal +". Leave empty to cancel");
            Console.SetCursorPosition(1, 2);
            Console.Write("New:");
            Console.CursorVisible = true;
            string s = Console.ReadLine();
            Console.CursorVisible = false;
            if (s == "" || s == null)
            {
                //cancelled
                return OldVal;
            }
            else return s;
        }
        private static Model ChangeModel(Model model)
        {
            bool run = true;
            List<KeyValuePair<string, Action>> Entries = new List<KeyValuePair<string, Action>>();
            Entries.Add(new KeyValuePair<string, Action>("", () => { model.ModelNumber = GetValue(model.ModelNumber); }));
            Entries.Add(new KeyValuePair<string, Action>("", () => { model.Packcodes[0].Code = GetValue(model.Packcodes[0].Code); }));
            Entries.Add(new KeyValuePair<string, Action>("", () => { model.Packcodes[0].ProductGroup = GetValue(model.Packcodes[0].ProductGroup); }));
            Entries.Add(new KeyValuePair<string, Action>("", () => { model.Packcodes[0].ProductCategory = GetValue(model.Packcodes[0].ProductCategory); }));
            Entries.Add(new KeyValuePair<string, Action>("", ()=> { run = false; }));
            Entries.Add(new KeyValuePair<string, Action>("", () => { throw new OperationCanceledException("Operation cancelled by the user."); }));
            int SelectedEntry = 0;
            while (run)
            {
                string[] Options = {
                    $"Model: {model.ModelNumber}",
                    $"Packcode: {model.Packcodes[0].Code}",
                    $"Packcode Product group: {model.Packcodes[0].ProductGroup}",
                    $"Packcode Product category: {model.Packcodes[0].ProductCategory}",
                    $"Save and exit",
                    $"Exit without saving"
                };
                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Clear();
                Console.SetCursorPosition(1, 1);
                Console.WriteLine("Choose which attribute to change");
                for(int i = 0;i<= Options.Length - 1; i++)
                {
                    Console.SetCursorPosition(1, 2+i);
                    if(SelectedEntry == i)
                    {
                        Console.Write("> ");
                    }
                    Console.WriteLine(Options[i]);
                }
                ConsoleKeyInfo input = Console.ReadKey();
                if (input.Key == ConsoleKey.Enter)
                {
                    //Select
                    Console.Clear();
                    Entries.ElementAt(SelectedEntry).Value.Invoke();
                }
                if (input.Key == ConsoleKey.DownArrow)
                {
                    SelectedEntry++;
                }
                if (input.Key == ConsoleKey.UpArrow)
                {
                    SelectedEntry--;
                }
                if (SelectedEntry <= -1)
                {
                    SelectedEntry = Entries.Count - 1;
                }
                if (SelectedEntry >= Entries.Count)
                {
                    SelectedEntry = 0;
                }
            }
            return model;
        }
        private static void DisplayModels()
        {
            Console.CursorVisible = false;
            Console.Clear();
            int ViewOffset = 0;
            bool Display = true;
            while (Display)
            {
                Console.Clear();
                //we will print header
                Console.ForegroundColor = ConsoleColor.Black;
                Console.BackgroundColor = ConsoleColor.White;
                string header = " Pos    Model           Packcode        Product Cat";
                Console.SetCursorPosition(0, 0);
                if (header.Length <= Console.WindowWidth)
                {
                    while (header.Length < Console.WindowWidth)
                    {
                        header += " ";
                    }
                }
                Console.Write(header);
                Console.ResetColor();
                for (int i = 0; i <= Console.WindowHeight - 3; i++)
                {
                    //we will print items
                    Console.SetCursorPosition(1, i + 1);
                    try
                    {
                        if(Modely[ViewOffset + i].Found) { Console.BackgroundColor = ConsoleColor.Green; }
                        Console.Write($"{String.Format("{0:DDD}",(ViewOffset+i+1).ToString())}\t{Modely[ViewOffset + i].ModelNumber}\t{Modely[ViewOffset + i].Packcodes[0]}\t{Modely[ViewOffset+i].Packcodes[0].ProductCategory}");
                        Console.ResetColor();
                    }catch(ArgumentOutOfRangeException ex)
                    {
                        Console.ResetColor();
                        break;
                    }
                }
                Console.ForegroundColor = ConsoleColor.Black;
                Console.BackgroundColor = ConsoleColor.White;
                Console.SetCursorPosition(0, Console.WindowHeight - 1);
                string help = "[UpArrow] Up   [DownArrow] Down   [Escape] Exit   [E] Change data";
                if (help.Length <= Console.WindowWidth)
                {
                    while (help.Length < Console.WindowWidth)
                    {
                        help += " ";
                    }
                }
                Console.Write(help);
                Console.ResetColor();
                Console.SetCursorPosition(0,0);
                ConsoleKeyInfo key = Console.ReadKey();
                switch (key.Key)
                {
                    case ConsoleKey.E:
                        //change data
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.SetCursorPosition(0, Console.WindowHeight - 1);
                        Console.Write("Please enter position, leave empty for cancel: ");
                        string pos = Console.ReadLine();
                        Int32.TryParse(pos, out int EditingPosition);
                        if(EditingPosition > 0 || EditingPosition != null)
                        {
                            //valid position, editing it...
                            Model model = Modely[EditingPosition - 1];
                            try
                            {
                                Model ChangedModel = ChangeModel(model);
                                Modely[EditingPosition - 1] = ChangedModel;
                            } catch(OperationCanceledException ex)
                            {
                                //continue, user cancelled the operation
                            }
                        }
                        else { //invalid position, continue
                        }
                        break;
                    case ConsoleKey.Escape:
                        Console.Clear();
                        Display = false;
                        break;
                    case ConsoleKey.UpArrow:
                        ViewOffset--;
                        break;
                    case ConsoleKey.DownArrow:
                        ViewOffset++;
                        break;
                    case ConsoleKey.End:
                        ViewOffset = Modely.Count - (Console.WindowHeight - 4);
                        break;
                    case ConsoleKey.Home:
                        ViewOffset = 0;
                        break;
                    case ConsoleKey.PageUp:
                        ViewOffset -= Console.WindowHeight - 2;
                        if (ViewOffset < 0)
                        {
                            ViewOffset = 0;
                        }
                        break;
                    case ConsoleKey.PageDown:
                        ViewOffset += Console.WindowHeight - 2;
                        if (ViewOffset >= (Modely.Count - (Console.WindowHeight - 5)))
                        {
                            ViewOffset = (Modely.Count - (Console.WindowHeight - 5));
                        }
                        break;
                }
                if (ViewOffset <= -1)
                {
                    if (Modely.Count >= Console.WindowHeight - 3)
                    {
                        ViewOffset = Modely.Count - (Console.WindowHeight - 4);
                    }
                    else
                    {
                        ViewOffset = Modely.Count - 1;
                    }
                }
                if(ViewOffset >= (Modely.Count - (Console.WindowHeight - 5)))
                {
                    ViewOffset = 0;
                }
            }
        }
        private static void CheckPackcodes(GuiSession session)
        {
            //Start transaction to assign devices
            string transaction = "ZLWCC70060";
            Debug($"{transaction} check data");
            session.StartTransaction(transaction);
            //Result List button
            GuiRadioButton RadioButtonResultList = (GuiRadioButton)session.ActiveWindow.FindById("/app/con[0]/ses[0]/wnd[0]/usr/radPA_DISP");
            RadioButtonResultList.Select();
            //List, get text fields
            GuiCTextField TextFieldCompany = (GuiCTextField)session.ActiveWindow.FindById("/app/con[0]/ses[0]/wnd[0]/usr/ctxtPA_BUKR2");
            GuiCTextField TextFieldModelLow = (GuiCTextField)session.ActiveWindow.FindById("/app/con[0]/ses[0]/wnd[0]/usr/ctxtSO_MODEL-LOW");
            GuiCTextField TextFieldModelHigh = (GuiCTextField)session.ActiveWindow.FindById("/app/con[0]/ses[0]/wnd[0]/usr/ctxtSO_MODEL-HIGH");
            GuiCTextField TextFieldPackLow = (GuiCTextField)session.ActiveWindow.FindById("/app/con[0]/ses[0]/wnd[0]/usr/ctxtSO_PACK-LOW");
            GuiCTextField TextFieldPackHigh = (GuiCTextField)session.ActiveWindow.FindById("/app/con[0]/ses[0]/wnd[0]/usr/ctxtSO_PACK-HIGH");
            GuiTextField TextFieldProductGroup = (GuiTextField)session.ActiveWindow.FindById("/app/con[0]/ses[0]/wnd[0]/usr/txtPA_SVCP2");
            GuiTextField TextFieldProductCategory = (GuiTextField)session.ActiveWindow.FindById("/app/con[0]/ses[0]/wnd[0]/usr/txtSO_SVCP2");
            //Fill 
            TextFieldCompany.Text = "C493";
            TextFieldModelLow.Text = Modely.First().ModelNumber;
            TextFieldModelHigh.Text = Modely.Last().ModelNumber;
            TextFieldPackLow.Text = "";
            TextFieldPackHigh.Text = "";
            TextFieldProductGroup.Text = "GT";
            TextFieldProductCategory.Text = "";
            //Enter button
            //GuiButton ButtonEnter = (GuiButton)session.ActiveWindow.FindById("/app/con[0]/ses[0]/wnd[0]/tbar[0]/btn[0]");

            //Find Execute button and press it
            GuiButton ButtonExecute = (GuiButton)session.ActiveWindow.FindById("/app/con[0]/ses[0]/wnd[0]/tbar[1]/btn[8]");
            ButtonExecute.SetFocus();
            ButtonExecute.Press();
            //Check
            GuiGridView GridView = (GuiGridView)session.ActiveWindow.FindById("/app/con[0]/ses[0]/wnd[0]/usr/cntlGRID1/shellcont/shell/shellcont[1]/shell");
            List<KeyValuePair<string, string>> PairsInView = new List<KeyValuePair<string, string>>();
            GridView.SetFocus();
            for (int i = 0; i <= GridView.RowCount - 1; i++)
            {
                GridView.SetCurrentCell(i, "PRODUCT");
                string PackNumber = GridView.GetCellValue(i, "PACK_NUMBER").Replace("C493-", "");
                string ModelNumber = GridView.GetCellValue(i, "PRODUCT");
                Model FoundPair = (Model)Modely.Find(x => (x.ModelNumber == ModelNumber && x.Packcodes[0].ToString() == PackNumber));
                if (FoundPair != null)
                {
                    FoundPair.Found = true;
                    PairsInView.Add(new KeyValuePair<string, string>(ModelNumber, PackNumber));
                    Debug($"Found {ModelNumber} {PackNumber}");
                }
                /*if (Model.IsPackcodePresent(PackNumber))
                {
                    Model FoundPair = (Model)Modely.Find(x => x.ModelNumber == ModelNumber);
                    if (FoundPair != null)
                    {
                        FoundPairs.Add(new KeyValuePair<string, string>(ModelNumber, PackNumber));
                        Debug($"Found {ModelNumber} {PackNumber}");
                    }
                }*/
            }
            Debug("Done!");
            Debug($"Found {PairsInView.Count}");
            Debug("Comparing with written data");
            Thread.Sleep(5000);
            List<KeyValuePair<string, string>> NotFoundPairs = new List<KeyValuePair<string, string>>();
            int PairedPairs = 0;
            for (int i = 0; i <= Modely.Count - 1; i++)
            {
                for (int j = 0; j <= Modely[i].Packcodes.Count - 1; j++)
                {
                    KeyValuePair<string, string> pair = new KeyValuePair<string, string>(Modely[i].ModelNumber, Modely[i].Packcodes[j].Code);
                    if (PairsInView.Contains(pair))
                    {
                        Debug($"{PairedPairs} / {PairsInView.Count} Found {pair.Key} {pair.Value}");
                        PairedPairs++;
                    }
                    else
                    {
                        if (!IsPackcodeWritten(session,pair.Key, pair.Value))
                        {
                            Error($"NOT Found {pair.Key} {pair.Value}");
                            NotFoundPairs.Add(pair);
                        }
                        else
                        {
                            Modely[i].Found = true;
                            Debug($"{PairedPairs} / {PairsInView.Count} Found {pair.Key} {pair.Value}");
                            PairedPairs++;
                        }
                    }
                }
            }
            Debug($"Paired {PairedPairs}. Not paired: {NotFoundPairs.Count}");
            StreamWriter sw = new StreamWriter("NotFound.txt");
            Debug($"Printing not paired pairs (here and to NotFound.txt):");
            for(int i = 0; i < NotFoundPairs.Count; i++)
            {
                string s = $"{NotFoundPairs[i].Key}\t{NotFoundPairs[i].Value}";
                Console.WriteLine(s);
                sw.WriteLine(s);
            }
            sw.Flush();
            sw.Close();
            Debug("EOF");
        }
        private static bool IsPackcodeWritten(GuiSession session, string Model, string Packcode)
        {
            bool Found = false;
            //Start transaction to assign devices
            string transaction = "ZLWCC70060";
            Debug($"{transaction} Check {Model} {Packcode}");
            session.StartTransaction(transaction);
            //Result List button
            GuiRadioButton RadioButtonResultList = (GuiRadioButton)session.ActiveWindow.FindById("/app/con[0]/ses[0]/wnd[0]/usr/radPA_DISP");
            RadioButtonResultList.Select();
            //List, get text fields
            GuiCTextField TextFieldCompany = (GuiCTextField)session.ActiveWindow.FindById("/app/con[0]/ses[0]/wnd[0]/usr/ctxtPA_BUKR2");
            GuiCTextField TextFieldModelLow = (GuiCTextField)session.ActiveWindow.FindById("/app/con[0]/ses[0]/wnd[0]/usr/ctxtSO_MODEL-LOW");
            GuiCTextField TextFieldModelHigh = (GuiCTextField)session.ActiveWindow.FindById("/app/con[0]/ses[0]/wnd[0]/usr/ctxtSO_MODEL-HIGH");
            GuiCTextField TextFieldPackLow = (GuiCTextField)session.ActiveWindow.FindById("/app/con[0]/ses[0]/wnd[0]/usr/ctxtSO_PACK-LOW");
            GuiCTextField TextFieldPackHigh = (GuiCTextField)session.ActiveWindow.FindById("/app/con[0]/ses[0]/wnd[0]/usr/ctxtSO_PACK-HIGH");
            GuiTextField TextFieldProductGroup = (GuiTextField)session.ActiveWindow.FindById("/app/con[0]/ses[0]/wnd[0]/usr/txtPA_SVCP2");
            GuiTextField TextFieldProductCategory = (GuiTextField)session.ActiveWindow.FindById("/app/con[0]/ses[0]/wnd[0]/usr/txtSO_SVCP2");
            //Fill 
            TextFieldCompany.Text = "C493";
            TextFieldModelLow.Text = Model;
            TextFieldModelHigh.Text = "";
            TextFieldPackLow.Text = Packcode;
            TextFieldPackHigh.Text = "";
            TextFieldProductGroup.Text = "GT";
            TextFieldProductCategory.Text = "";
            //Enter button
            //GuiButton ButtonEnter = (GuiButton)session.ActiveWindow.FindById("/app/con[0]/ses[0]/wnd[0]/tbar[0]/btn[0]");

            //Find Execute button and press it
            GuiButton ButtonExecute = (GuiButton)session.ActiveWindow.FindById("/app/con[0]/ses[0]/wnd[0]/tbar[1]/btn[8]");
            ButtonExecute.SetFocus();
            ButtonExecute.Press();
            //Check
            GuiGridView GridView = (GuiGridView)session.ActiveWindow.FindById("/app/con[0]/ses[0]/wnd[0]/usr/cntlGRID1/shellcont/shell/shellcont[1]/shell");
            GridView.SetFocus();
            for (int i = 0; i <= GridView.RowCount - 1; i++)
            {
                GridView.SetCurrentCell(i, "PRODUCT");
                string PackNumber = GridView.GetCellValue(i, "PACK_NUMBER").Replace("C493-", "");
                string ModelNumber = GridView.GetCellValue(i, "PRODUCT");
                Model FoundPair = (Model)Modely.Find(x => (x.ModelNumber == ModelNumber && x.Packcodes[0].ToString() == PackNumber));
                if (FoundPair != null)
                {
                    Found = true;
                    Debug($"Found {ModelNumber} {PackNumber}");
                    break;
                }
            }
            return Found;
        }
        private static void ApplyPackcodes(GuiSession session, Model model)
        {
            //Start transaction to assign devices
            string transaction = "ZLWCC70060";
            Debug($"{transaction} assign {model.ModelNumber}");
            //model.Packcodes.ForEach((pkg)=> { Debug($"\t{pkg}"); });
            session.StartTransaction(transaction);
            //Assign
            GuiRadioButton RadioButtonAssign = (GuiRadioButton)session.ActiveWindow.FindById("/app/con[0]/ses[0]/wnd[0]/usr/radPA_ASSI");
            RadioButtonAssign.Select();
            GuiCTextField TextFieldCompany = (GuiCTextField)session.ActiveWindow.FindById("/app/con[0]/ses[0]/wnd[0]/usr/ctxtPA_BUKRS");
            TextFieldCompany.Text = "C493";
            GuiCTextField TextFieldModel = (GuiCTextField)session.ActiveWindow.FindById("/app/con[0]/ses[0]/wnd[0]/usr/ctxtPA_MODEL");
            TextFieldModel.Text = model.ModelNumber;
            GuiCTextField TextFieldProductGrp = (GuiCTextField)session.ActiveWindow.FindById("/app/con[0]/ses[0]/wnd[0]/usr/ctxtPA_SVCPG");
            TextFieldProductGrp.Text = model.Packcodes[0].ProductGroup;
            GuiCTextField TextFieldProductCat = (GuiCTextField)session.ActiveWindow.FindById("/app/con[0]/ses[0]/wnd[0]/usr/ctxtSO_SVCPC");
            TextFieldProductCat.Text = model.Packcodes[0].ProductCategory;
            //Find Execute button and press it
            GuiButton ButtonExecute = (GuiButton)session.ActiveWindow.FindById("/app/con[0]/ses[0]/wnd[0]/tbar[1]/btn[8]");
            ButtonExecute.SetFocus();
            ButtonExecute.Press();
            //Wait for data, find gridview
            GuiGridView GridView = (GuiGridView)session.ActiveWindow.FindById("/app/con[0]/ses[0]/wnd[0]/usr/cntlG_CONTAINER/shellcont/shell");
            GridView.SetFocus();
            //Debug(GridView.CurrentCellColumn);
            //find rows which contain needed codes
            List<int> RowsWithCodes = new List<int>();
            for (int i = 0; i <= GridView.RowCount - 1; i++)
            {
                string val = GridView.GetCellValue(i, "PACK_NUMBER");
                for (int j = 0; j <= model.Packcodes.Count - 1; j++)
                {
                    if (val.Contains(model.Packcodes[j].Code))
                    {
                        //found code
                        RowsWithCodes.Add(i);
                        GridView.SelectedRows = (i).ToString();
                    }
                }
            }
            if (RowsWithCodes.Count == model.Packcodes.Count)
            {
                //we found every code, all is well
            }
            else
            {
                //something wasnt found!
                Error($"Packcode not found! {model.ModelNumber}");
            }
            //GuiButton ButtonUnAssignPackages = (GuiButton)session.ActiveWindow.FindById("/app/con[0]/ses[0]/wnd[0]/tbar[1]/btn[14]");
            for (int i = 0; i <= RowsWithCodes.Count - 1; i++)
            {
                GridView.SelectedRows = RowsWithCodes[i].ToString();
                if (GridView.SelectedRows == "") { }
                else
                {
                    //Finally assign packages
                    GuiButton ButtonAssignPackages = (GuiButton)session.ActiveWindow.FindById("/app/con[0]/ses[0]/wnd[0]/tbar[1]/btn[13]");
                    Debug($"\t{model.Packcodes[i].ToString()}");
                    ButtonAssignPackages.Press();
                    try
                    {
                        //C493-P-GT-NKXSS1LV already assigned to
                        GuiStatusPane StatusPaneAlreadyAssigned = (GuiStatusPane)session.ActiveWindow.FindById("/app/con[0]/ses[0]/wnd[0]/sbar/pane[0]");
                        if (StatusPaneAlreadyAssigned.Text.Contains("already assigned to"))
                        {
                            //need to unassign that specific package for this specific model
                            Regex RegexAlreadyAssigned = new Regex(@"C493\-([A-Z0-9\-]{1,})");
                            Match MatchAlreadyAssigned = RegexAlreadyAssigned.Match(StatusPaneAlreadyAssigned.Text);
                            //create new model object
                            Model UnassignModel = new Model(model.MktName, model.ModelNumber);
                            UnassignModel.Packcodes.Clear();
                            UnassignModel.Packcodes.Add(new Packcode(MatchAlreadyAssigned.Groups[1].Value));
                            //throw him into unassign fn
                            UnassignPackage(session, UnassignModel);
                            //take this object with this specific package and assign it
                            Model reassignmodel = new Model(model.MktName, model.ModelNumber);
                            reassignmodel.Packcodes.Clear();
                            reassignmodel.Packcodes.Add(new Packcode(model.Packcodes[i].ToString()));
                            ApplyPackcodes(session, model);
                            //return everything back, so this program can continue
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message == "The control could not be found by id.")
                        {
                            //successfully pressed button, no popup
                        }
                        else
                        {
                            Error($"CRITICAL ERROR! {model.ModelNumber} {i} on line 140");
                        }
                    }
                    try
                    {
                        //Weird Information window Select line please.
                        GuiTextField TextFieldSelectLine = (GuiTextField)session.ActiveWindow.FindById("/app/con[0]/ses[0]/wnd[1]/usr/txtMESSTXT1");
                        if (TextFieldSelectLine.Text.Contains("Select line please."))
                        {
                            //weird popup appeared
                            GuiButton ButtonContinue = (GuiButton)session.ActiveWindow.FindById("/app/con[0]/ses[0]/wnd[1]/tbar[0]/btn[0]");
                            ButtonContinue.Press();
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message == "The control could not be found by id.")
                        {
                            //successfully pressed button, no popup
                        }
                        else
                        {
                            Error($"CRITICAL ERROR! {model.ModelNumber} {i} on line 162");
                        }
                    }
                    try
                    {
                        //are you sure?
                        GuiTextField TextFieldAreYouSure = (GuiTextField)session.ActiveWindow.FindById("/app/con[0]/ses[0]/wnd[1]/usr/txtSPOP-TEXTLINE1");
                        if (TextFieldAreYouSure.Text.Contains("Are you sure to assign to packages"))
                        {
                            //It really is are you sure window
                            //we want to press yes
                            GuiButton ButtonYes = (GuiButton)session.ActiveWindow.FindById("/app/con[0]/ses[0]/wnd[1]/usr/btnSPOP-OPTION1");
                            GuiButton ButtonNo = (GuiButton)session.ActiveWindow.FindById("/app/con[0]/ses[0]/wnd[1]/usr/btnSPOP-OPTION2");
                            GuiButton ButtonCancel = (GuiButton)session.ActiveWindow.FindById("/app/con[0]/ses[0]/wnd[1]/usr/btnSPOP-OPTION_CAN");
                            ButtonYes.Press();
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message == "The control could not be found by id.")
                        {
                            //successfully pressed button, no popup
                        }
                        else
                        {
                            Error($"CRITICAL ERROR! {model.ModelNumber} {i} on line 186");
                        }
                    }
                }
            }
            //check if all codes are assigned
            GuiStatusPane StatusPane = (GuiStatusPane)session.ActiveWindow.FindById("/app/con[0]/ses[0]/wnd[0]/sbar/pane[0]");
            string s = StatusPane.Text;
            //Total  9 Data Selected(Assign:  5, Unassign:  4)
            if (s.Contains("Assign: "))
            {
                //successfully assigned some packs
                Regex AssignRegex = new Regex(@"\(Assign:[ \t]*([0-9]*),");
                Match AssignMatch = AssignRegex.Match(s);
                if (Int32.Parse(AssignMatch.Groups[1].Value) >= model.Packcodes.Count)
                {
                    //successfully assigned all codes
                    Debug($"{model.ModelNumber} assigned!");
                }
                else
                {
                    Error($"Could not assign all codes!");
                    Error($"\t{model.ModelNumber}");
                    model.Packcodes.ForEach((Packcode) => { Error($"\t\t{Packcode.Code}"); });
                    //not successful
                }
            }
            GuiButton ButtonEnter = (GuiButton)session.ActiveWindow.FindById("/app/con[0]/ses[0]/wnd[0]/tbar[0]/btn[0]");
            ButtonEnter.SetFocus();
            ButtonEnter.Press();
            Thread.Sleep(2000);
        }
        private static void UnassignPackage(GuiSession session, Model model)
        {
            //Start transaction to assign devices
            string transaction = "ZLWCC70060";
            Debug($"{transaction} Unassign {model.ModelNumber}");
            model.Packcodes.ForEach((pkg) => { Debug($"\t{pkg}"); });
            session.StartTransaction(transaction);
            //Assign
            GuiRadioButton RadioButtonAssign = (GuiRadioButton)session.ActiveWindow.FindById("/app/con[0]/ses[0]/wnd[0]/usr/radPA_ASSI");
            RadioButtonAssign.Select();
            GuiCTextField TextFieldCompany = (GuiCTextField)session.ActiveWindow.FindById("/app/con[0]/ses[0]/wnd[0]/usr/ctxtPA_BUKRS");
            TextFieldCompany.Text = "C493";
            GuiCTextField TextFieldModel = (GuiCTextField)session.ActiveWindow.FindById("/app/con[0]/ses[0]/wnd[0]/usr/ctxtPA_MODEL");
            TextFieldModel.Text = model.ModelNumber;
            GuiCTextField TextFieldProductGrp = (GuiCTextField)session.ActiveWindow.FindById("/app/con[0]/ses[0]/wnd[0]/usr/ctxtPA_SVCPG");
            TextFieldProductGrp.Text = model.Packcodes[0].ProductGroup;
            GuiCTextField TextFieldProductCat = (GuiCTextField)session.ActiveWindow.FindById("/app/con[0]/ses[0]/wnd[0]/usr/ctxtSO_SVCPC");
            TextFieldProductCat.Text = model.Packcodes[0].ProductCategory;
            //Find Execute button and press it
            GuiButton ButtonExecute = (GuiButton)session.ActiveWindow.FindById("/app/con[0]/ses[0]/wnd[0]/tbar[1]/btn[8]");
            ButtonExecute.SetFocus();
            ButtonExecute.Press();
            //Wait for data, find gridview
            GuiGridView GridView = (GuiGridView)session.ActiveWindow.FindById("/app/con[0]/ses[0]/wnd[0]/usr/cntlG_CONTAINER/shellcont/shell");
            GridView.SetFocus();
            //find rows which contain needed codes, and unassign them
            int UnassignCount = 0;
            for (int i = 0; i <= GridView.RowCount - 1; i++)
            {
                string val = GridView.GetCellValue(i, "PACK_NUMBER");
                for (int j = 0; j <= model.Packcodes.Count - 1; j++)
                {
                    if (val.Contains(model.Packcodes[j].Code))
                    {
                        //found code
                        GridView.SelectedRows = (i).ToString();
                        GuiButton ButtonUnAssignPackages = (GuiButton)session.ActiveWindow.FindById("/app/con[0]/ses[0]/wnd[0]/tbar[1]/btn[14]");
                        ButtonUnAssignPackages.Press();
                        try
                        {
                            //are you sure?
                            GuiTextField TextFieldAreYouSure = (GuiTextField)session.ActiveWindow.FindById("/app/con[0]/ses[0]/wnd[1]/usr/txtSPOP-TEXTLINE1");
                            if (TextFieldAreYouSure.Text.Contains("Are you sure to unassign from packages"))
                            {
                                //It really is are you sure window
                                //we want to press yes
                                GuiButton ButtonYes = (GuiButton)session.ActiveWindow.FindById("/app/con[0]/ses[0]/wnd[1]/usr/btnSPOP-OPTION1");
                                GuiButton ButtonNo = (GuiButton)session.ActiveWindow.FindById("/app/con[0]/ses[0]/wnd[1]/usr/btnSPOP-OPTION2");
                                GuiButton ButtonCancel = (GuiButton)session.ActiveWindow.FindById("/app/con[0]/ses[0]/wnd[1]/usr/btnSPOP-OPTION_CAN");
                                ButtonYes.Press();
                            }
                        }
                        catch (Exception ex)
                        {
                            if (ex.Message == "The control could not be found by id.")
                            {
                                //successfully pressed button, no popup
                            }
                            else
                            {
                                Error($"CRITICAL ERROR! {model.ModelNumber} {i} on line 186");
                            }
                        }
                        UnassignCount++;
                    }
                }
            }
            if (UnassignCount == model.Packcodes.Count)
            {
                //we found every code, all is well
            }
            else
            {
                //something wasnt found!
                Error($"Packcode not found! {model.ModelNumber}");
            }
        }
        private static void GetIds(GuiSession session)
        {
            Console.WriteLine("Please Focus");
            Thread.Sleep(5000);
            //Focusing on the item prints out the info to console
            while (true)
            {
                GuiVComponent comp = session.ActiveWindow.GuiFocus;
                Debug($"{comp.Type} {comp.Id} {comp.Name} {comp.Tooltip}");
                Thread.Sleep(2000);
            }
        }
        private static void LoadModels()
        {
            StreamReader ReaderSKUs = new StreamReader("SKUS.txt");
            while (true)
            {
                string Line = ReaderSKUs.ReadLine();
                if (Line.StartsWith("//"))
                {
                    //Is a comment, do not use!
                }
                else
                {
                    Model model = new Model(Line.Split(",")[1], Line.Split(",")[0]);
                    if (model.Packcodes.Count == 0)
                    {
                        //Empty packcodes, do not add!
                        Debug($"Omitting {model.ModelNumber}");
                    }
                    else
                    {
                        Modely.Add(model);
                    }
                }
                if (ReaderSKUs.EndOfStream == true)
                {
                    break;
                }
            }
            Debug("Models loaded!");
            ReaderSKUs.Dispose();
        }
        public static void Debug(string s)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("DEBUG [" + DateTime.Now.ToString("ddMMyyyy HHmmss") + "] ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"{s}");
        }
        public static void Error(string s)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("ERROR [" + DateTime.Now.ToString("ddMMyyyy HHmmss") + "] ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"{s}");
        }
    }
}
