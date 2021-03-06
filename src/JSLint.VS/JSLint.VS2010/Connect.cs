using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Xml.Serialization;

using EnvDTE;
using EnvDTE80;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Text;
using JSLint.VS2010.LinterBridge;
using JSLint.VS2010.OptionClasses;
using JSLint.VS2010.OptionsUI;
using JSLint.VS2010.OptionClasses.OptionProviders;
using Microsoft.VisualStudio.OLE.Interop;

namespace JSLint.VS2010.VS2010
{
	static class GuidList
	{
		public const string guidPkgString = "531da20f-aec5-4382-a317-0f70a217a572";
		public const string guidSourceEditorCmdSetString = "36c9360d-beb4-4ee0-9e7c-75264224c59c";

		public const string guidSourceEditorFragmentCmdSetString = "45b15f14-e401-462c-8eb0-5fcab4cc3195";
		public const string guidSolutionItemCmdSetString = "d9032052-cbd8-4f67-8352-e183f74e4071";
		public const string guidSolutionFolderNodeCmdSetString = "11ecda11-e5ee-4e59-a4e1-ef9edec80d8f";
		public const string guidOptionsCmdSetString = "b7fe589c-403a-4bfa-88e2-795a77b2d5b3";
		public const string guidErrorListCmdString = "284b5171-6b47-4cbf-a617-ab8b7d113725";
		public const string guidSolutionNodeCmdString = "72a924bc-cf95-456b-bc68-edf0b2022c7d";

		public static readonly Guid guidSourceEditorCmdSet = new Guid(guidSourceEditorCmdSetString);
		public static readonly Guid guidSourceEditorFragmentCmdSet = new Guid(guidSourceEditorFragmentCmdSetString);
		public static readonly Guid guidSolutionItemCmdSet = new Guid(guidSolutionItemCmdSetString);
		public static readonly Guid guidSolutionFolderNodeCmdSet = new Guid(guidSolutionFolderNodeCmdSetString);
		public static readonly Guid guidOptionsCmdSet = new Guid(guidOptionsCmdSetString);
		public static readonly Guid guidErrorListCmdSet = new Guid(guidErrorListCmdString);
		public static readonly Guid guidSolutionNodeCmdSet = new Guid(guidSolutionNodeCmdString);
	}

	static class PkgCmdIDList
	{
		public const uint lint = 0x100;
		public const uint options = 0x100;
		public const uint wipeerrors = 0x100;
		public const uint exclude = 0x101;
		public const uint excludeFolder = 0x100;
		public const uint globals = 0x102;
		public const uint addconfig = 0x103;
		public const uint editconfig = 0x104;
		public const uint removeconfig = 0x105;
	}

	[PackageRegistration(UseManagedResourcesOnly = true)]
	// This attribute is used to register the informations needed to show the this package
	// in the Help/About dialog of Visual Studio.
	[InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
	// This attribute is needed to let the shell know that this package exposes some menus.
	[ProvideMenuResource("Menus.ctmenu", 1)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.NoSolution_string)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.SolutionExists_string)]
	[ProvideSolutionProps(Connect2.SolutionPropertiesKeyName)]
	[Guid(GuidList.guidPkgString)]
	public sealed class Connect2 : Package, IVsPersistSolutionProps, IDisposable
	{
		private DTE2 _dte2;
		
		private SolutionEvents _solutionEvents; // needs to be a field otherwise gc'd
		private BuildEvents _buildEvents; // needs to be a field otherwise gc'd
		private DocumentEvents _docEvents; // needs to be a field otherwise gc'd

		private vsBuildScope _buildScope;
		private vsBuildAction _buildAction;

		private ErrorListHelper _errorListHelper;
		private JSLinter _linter = new JSLinter();

		private int _errorCount;
		private const int Threshold = 1000;

		private Dictionary<string, List<string>> _skippedNodes =
			new Dictionary<string, List<string>>(8);

		private bool _usingSolutionOptions = false;
		private bool _hasDirtySolutionProperties = false;
		private String _solutionConfigurationPath;

		private const String SolutionPropertiesKeyName = "JSLint";
		private static readonly String JsLintOptionsFileName = "JSLintOptions.xml";
		private static readonly String GlobalOptionsPath = Utility.GetFilename(JsLintOptionsFileName);

        private static readonly char[] DirectorySeparators = new[] { Path.DirectorySeparatorChar };


		public Connect2()
		{
		}

		protected override void Initialize()
		{
			base.Initialize();

			OptionsProviderRegistry.PushOptionsProvider(new FileOptionsProvider("Global", GlobalOptionsPath));

			OptionsProviderRegistry.ReloadCurrent();

			_dte2 = GetService(typeof(DTE)) as DTE2;


			_solutionEvents = _dte2.Events.SolutionEvents;
			_buildEvents = _dte2.Events.BuildEvents;
			_docEvents = _dte2.Events.DocumentEvents;
			_errorListHelper = new ErrorListHelper(_dte2);

			// Add our command handlers for menu (commands must exist in the .vsct file)
			OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
			if (null != mcs)
			{
				// Source Editor: JS Lint
				CommandID sourceEditorLintCmdID = new CommandID(GuidList.guidSourceEditorCmdSet, (int)PkgCmdIDList.lint);
				MenuCommand sourceEditorLintMenuItem = new MenuCommand(LintSourceEditorCmdCallback, sourceEditorLintCmdID);
				mcs.AddCommand(sourceEditorLintMenuItem);

				// Source Editor: JS Lint Fragment
				CommandID sourceEditorFragmentLintCmdID = new CommandID(GuidList.guidSourceEditorFragmentCmdSet, (int)PkgCmdIDList.lint);
				OleMenuCommand sourceEditorFragmentLintMenuItem = new OleMenuCommand(LintSourceEditorFragmentItemCmdCallback, sourceEditorFragmentLintCmdID);
				sourceEditorFragmentLintMenuItem.BeforeQueryStatus += sourceEditorFragmentLintMenuItem_BeforeQueryStatus;
				mcs.AddCommand(sourceEditorFragmentLintMenuItem);

				// Solution Explorer: JS Lint
				CommandID solutionItemCmdID = new CommandID(GuidList.guidSolutionItemCmdSet, (int)PkgCmdIDList.lint);
				OleMenuCommand solutionItemMenuItem = new OleMenuCommand(LintSolutionItemCmdCallback, solutionItemCmdID);
				solutionItemMenuItem.BeforeQueryStatus += solutionItemMenuItem_BeforeQueryStatus;
				mcs.AddCommand(solutionItemMenuItem);

				// Solution Explorer: Skip File
				CommandID solutionItemSkipCmdID = new CommandID(GuidList.guidSolutionItemCmdSet, (int)PkgCmdIDList.exclude);
				OleMenuCommand solutionItemSkipMenuItem = new OleMenuCommand(LintSolutionItemSkipCmdCallback, solutionItemSkipCmdID);
				solutionItemSkipMenuItem.BeforeQueryStatus += solutionItemSkipMenuItem_BeforeQueryStatus;
				mcs.AddCommand(solutionItemSkipMenuItem);

				// Solution Explorer: Skip Folder
				CommandID solutionFolderNodeSkipCmdID = new CommandID(GuidList.guidSolutionFolderNodeCmdSet, (int)PkgCmdIDList.excludeFolder);
				OleMenuCommand solutionFolderNodeSkipMenuItem = new OleMenuCommand(LintSolutionFolderNodeSkipCmdCallback, solutionFolderNodeSkipCmdID);
				solutionFolderNodeSkipMenuItem.BeforeQueryStatus += solutionFolderNodeSkipMenuItem_BeforeQueryStatus;
				mcs.AddCommand(solutionFolderNodeSkipMenuItem);

				// Solution Explorer: Predefined global variables
				CommandID solutionItemGlobalsCmdID = new CommandID(GuidList.guidSolutionItemCmdSet, (int)PkgCmdIDList.globals);
				OleMenuCommand solutionItemGlobalsMenuItem = new OleMenuCommand(LintSolutionItemGlobalsCmdCallback, solutionItemGlobalsCmdID);
				solutionItemGlobalsMenuItem.BeforeQueryStatus += solutionItemGlobalsMenuItem_BeforeQueryStatus;
				mcs.AddCommand(solutionItemGlobalsMenuItem);
                
				// Source Editor: Predefined global variables
				CommandID sourceEditorGlobalsCmdID = new CommandID(GuidList.guidSourceEditorCmdSet, (int)PkgCmdIDList.globals);
				MenuCommand sourceEditorGlobalsMenuItem = new MenuCommand(LintSourceEditorGlobalsCmdCallback, sourceEditorGlobalsCmdID);
				mcs.AddCommand(sourceEditorGlobalsMenuItem);

				// Error List: Clear JS Lint Errors
				CommandID errorListCmdID = new CommandID(GuidList.guidErrorListCmdSet, (int)PkgCmdIDList.wipeerrors);
				OleMenuCommand errorListMenuItem = new OleMenuCommand(ErrorListCmdCallback, errorListCmdID);
				errorListMenuItem.BeforeQueryStatus += errorListMenuItem_BeforeQueryStatus;
				mcs.AddCommand(errorListMenuItem);

				// Solution Node: Add Config
				CommandID addConfigCmdID = new CommandID(GuidList.guidSolutionNodeCmdSet, (int)PkgCmdIDList.addconfig);
				OleMenuCommand addConfigMenuItem = new OleMenuCommand(AddSolutionOptionsFileCmdCallback, addConfigCmdID);
				addConfigMenuItem.BeforeQueryStatus += new EventHandler(addConfigMenuItem_BeforeQueryStatus);
				mcs.AddCommand(addConfigMenuItem);

				// Solution Node: Edit Config
				CommandID editConfigCmdID = new CommandID(GuidList.guidSolutionNodeCmdSet, (int)PkgCmdIDList.editconfig);
				OleMenuCommand editConfigMenuItem = new OleMenuCommand(EditSolutionOptionsFileCmdCallback, editConfigCmdID);
				editConfigMenuItem.BeforeQueryStatus += new EventHandler(editOrRemoveConfigMenuItem_BeforeQueryStatus);
				mcs.AddCommand(editConfigMenuItem);

				// Solution Node: Remove Config
				CommandID removeConfigCmdID = new CommandID(GuidList.guidSolutionNodeCmdSet, (int)PkgCmdIDList.removeconfig);
				OleMenuCommand removeConfigMenuItem = new OleMenuCommand(RemoveSolutionOptionsFileCmdCallback, removeConfigCmdID);
				removeConfigMenuItem.BeforeQueryStatus += new EventHandler(editOrRemoveConfigMenuItem_BeforeQueryStatus);
				mcs.AddCommand(removeConfigMenuItem);

				// Main Menu: JSLint Options
				CommandID optionsCmdID = new CommandID(GuidList.guidOptionsCmdSet, (int)PkgCmdIDList.options);
				MenuCommand optionsMenuItem = new MenuCommand(OptionsCmdCallback, optionsCmdID);
				mcs.AddCommand(optionsMenuItem);
			}

			//solution events
			_solutionEvents.Opened += solutionEvents_Opened;
			_solutionEvents.AfterClosing += solutionEvents_AfterClosing;

			// build events
			_buildEvents.OnBuildBegin += buildEvents_OnBuildBegin;
			_buildEvents.OnBuildProjConfigBegin += buildEvents_OnBuildProjConfigBegin;

			//document events
			_docEvents.DocumentSaved += DocumentEvents_DocumentSaved;
			
		}

		private void addConfigMenuItem_BeforeQueryStatus(object sender, EventArgs e)
		{
			OleMenuCommand menuCommand = sender as OleMenuCommand;
			if (menuCommand != null)
			{
				menuCommand.Visible = !_usingSolutionOptions;
			}
		}

		private void editOrRemoveConfigMenuItem_BeforeQueryStatus(object sender, EventArgs e)
		{
			OleMenuCommand menuCommand = sender as OleMenuCommand;
			if (menuCommand != null)
			{
				menuCommand.Visible = _usingSolutionOptions;
			}
		}

		private void solutionEvents_Opened()
		{
			if (_usingSolutionOptions)
			{
				if (File.Exists(_solutionConfigurationPath))
				{
					var newProvider = new FileOptionsProvider("Solution", _solutionConfigurationPath);
					OptionsProviderRegistry.PushOptionsProvider(newProvider);
					_usingSolutionOptions = true;
				}
				else
				{
					_usingSolutionOptions = false;
				}
			}
		}

		private void solutionEvents_AfterClosing()
		{
			if (_usingSolutionOptions)
			{
				OptionsProviderRegistry.PopOptionsProvider();
			}
			_usingSolutionOptions = false;
		}

		void DocumentEvents_DocumentSaved(Document document)
		{
			Options currentOptions = OptionsProviderRegistry.CurrentOptions;
			if (currentOptions.RunOnSave && document != null)
			{
				IncludeFileType fileType = GetFileType(document.FullName);
				if ((fileType & currentOptions.SaveFileTypes) > 0)
				{
					ResetErrorCount();
					SuspendErrorList();

					bool bDontCare;
					ClearErrors(document.FullName);
					AnalyzeFile(document.FullName, out bDontCare);
					
					ResumeErrorList(false);
				}
			}
		}

		private void solutionItemMenuItem_BeforeQueryStatus(object sender, EventArgs e)
		{
			OleMenuCommand menuCommand = sender as OleMenuCommand;
			if (menuCommand != null)
			{
				menuCommand.Visible = ActiveUIHierarchyItems.Cast<UIHierarchyItem>()
															.Select(item => GetFileType(item.Name))
															.All(type => type != IncludeFileType.None && type != IncludeFileType.Folder);
			}
		}

		private void solutionItemSkipMenuItem_BeforeQueryStatus(object sender, EventArgs e)
		{
			OleMenuCommand menuCommand = sender as OleMenuCommand;
			if (menuCommand != null)
			{
				var activeItems = ActiveUIHierarchyItems.Cast<UIHierarchyItem>();

				var buildFileTypes = OptionsProviderRegistry.CurrentOptions.BuildFileTypes;

				menuCommand.Visible = activeItems.Select(item => GetFileType(item.Name))
													.All(type => buildFileTypes.HasFlag(type) && (type != IncludeFileType.None));

				menuCommand.Checked = false;
				menuCommand.Enabled = true;
				foreach (UIHierarchyItem item in activeItems)
				{
                    ProjectItem projItem = (ProjectItem)item.Object;
                    if (SolutionFolderKind.Equals(projItem.ContainingProject.Kind, StringComparison.Ordinal))
                    {
                        menuCommand.Visible = false;
                        return;
                    }
					bool skippedByFolder;
					if (IsNodeSkipped(projItem, out skippedByFolder))
					{
						menuCommand.Checked = true;
						if (skippedByFolder)
						{
							menuCommand.Enabled = false;
						}

						return;
					}
				}
			}
		}

		private void solutionFolderNodeSkipMenuItem_BeforeQueryStatus(object sender, EventArgs e)
		{
			OleMenuCommand menuCommand = sender as OleMenuCommand;
			menuCommand.Visible = true;
			if (menuCommand != null)
			{
				var activeItems = ActiveUIHierarchyItems.Cast<UIHierarchyItem>();

				menuCommand.Checked = activeItems.Select(item => (ProjectItem)item.Object)
													.Any(projItem => IsNodeSkipped(projItem));

			}
		}

		private void solutionItemGlobalsMenuItem_BeforeQueryStatus(object sender, EventArgs e)
		{
			OleMenuCommand menuCommand = sender as OleMenuCommand;
			if (menuCommand != null)
			{
				var activeItems = ActiveUIHierarchyItems;
				if (activeItems.Length != 1)
				{
					menuCommand.Visible = false;

					return;
				}

				menuCommand.Visible = GetFileType(((ProjectItem)ActiveUIHierarchyItem.Object).Name) == IncludeFileType.JS;
			}
		}

		private void sourceEditorFragmentLintMenuItem_BeforeQueryStatus(object sender, EventArgs e)
		{
			OleMenuCommand menuCommand = sender as OleMenuCommand;
			if (menuCommand != null)
			{
				string selection = ActiveTextDocument.Selection.Text;

				menuCommand.Visible = selection.Length > 0 && (GetFileType(_dte2.ActiveDocument.FullName, selection) != IncludeFileType.None);
			}
		}

		private void errorListMenuItem_BeforeQueryStatus(object sender, EventArgs e)
		{
			OleMenuCommand menuCommand = sender as OleMenuCommand;
			var currentOptions = OptionsProviderRegistry.CurrentOptions;
			if (menuCommand != null)
			{
				menuCommand.Visible = currentOptions.ErrorCategory.IsTaskError() &&
					_errorListHelper != null &&
					_errorListHelper.ErrorCount > 0;
			}
		}

		private void EditSolutionOptionsFileCmdCallback(object sender, EventArgs e)
		{
			OpenOptionsDialog();
		}

		private void RemoveSolutionOptionsFileCmdCallback(object sender, EventArgs e)
		{
			ProjectItem projItem = _dte2.Solution.FindProjectItem(_solutionConfigurationPath);
			if (projItem != null)
				projItem.Remove();

			OptionsProviderRegistry.PopOptionsProvider();
			_usingSolutionOptions = false;
			_solutionConfigurationPath = null;
			_hasDirtySolutionProperties = true;
		}

        private static string GetFileName(ProjectItem projItem)
        {
            //get round some really weird 2012 behaviour
            // where Filenames[0] causes an exception
            // and Filenames[1] does not..
            // to reproduce add a solution folder and put a js file in it.
            try
            {
                return projItem.FileNames[1];
            }
            catch
            {
                try
                {
                    return projItem.FileNames[0];
                }
                catch
                {
                }
            }
            return "";
        }

		private void LintSolutionItemCmdCallback(object sender, EventArgs e)
		{
			ResetErrorCount();
			SuspendErrorList();

			foreach (var item in ActiveUIHierarchyItems)
			{
				ProjectItem projItem = (ProjectItem)((UIHierarchyItem)item).Object;
                var fileName = GetFileName(projItem);

				ClearErrors(fileName);
				bool reachedTreshold;
                AnalyzeFile(fileName, out reachedTreshold);
				if (reachedTreshold)
				{
					break;
				}
			}

			ResumeErrorList();
		}

		private void LintSolutionItemSkipCmdCallback(object sender, EventArgs e)
		{
			var buildFileTypes = OptionsProviderRegistry.CurrentOptions.BuildFileTypes;
			
			var buildableFiles = ActiveUIHierarchyItems.Cast<UIHierarchyItem>()
													.Select(item => item.Object)
													.Cast<ProjectItem>()
													.Select(projItem => new
													{
														Item = projItem,
														FileType = GetFileType(projItem.Name)
													})
													.Where(c => buildFileTypes.HasFlag(c.FileType))
													.Select(c => c.Item);

			foreach (var item in buildableFiles)
			{
				ToggleSkip(item);
			}
		}

		private void LintSolutionFolderNodeSkipCmdCallback(object sender, EventArgs e)
		{
			var activeProjectItems = ActiveUIHierarchyItems.Cast<UIHierarchyItem>()
															.Select(item => item.Object)
															.Cast<ProjectItem>();
			foreach (ProjectItem projItem in activeProjectItems)
			{
				ToggleSkip(projItem);
			}
		}

		private void GotoGlobals(TextDocument doc)
		{
			doc.Selection.MoveToLineAndOffset(1, 1);
			if (doc.Selection.FindText("/*global"))
			{
				doc.Selection.SelectLine();
			}
			else
			{
				doc.CreateEditPoint().Insert("/*global _comma_separated_list_of_variables_*/\r\n");
				doc.Selection.MoveToLineAndOffset(1, 1);
				doc.Selection.FindText("_comma_separated_list_of_variables_");
			}
		}

		private void LintSolutionItemGlobalsCmdCallback(object sender, EventArgs e)
		{
			var item = ActiveUIHierarchyItem;
			ProjectItem projItem = (ProjectItem)((UIHierarchyItem)item).Object;
			projItem.Open().Document.Activate();

			GotoGlobals(ActiveTextDocument);
		}

		private void LintSourceEditorGlobalsCmdCallback(object sender, EventArgs e)
		{
			GotoGlobals(ActiveTextDocument);
		}

		private void LintSourceEditorFragmentItemCmdCallback(object sender, EventArgs e)
		{
			if (ActiveTextDocument.Selection.Text.Length > 0)
			{
				SuspendErrorList();

				string filename = _dte2.ActiveDocument.FullName;
				ClearErrors(filename);

				AnalyzeFragment(ActiveTextDocument.Selection.Text, filename, ActiveTextDocument.Selection.TopPoint.Line, ActiveTextDocument.Selection.TopPoint.DisplayColumn);

				ResumeErrorList();
			}
		}

		private void LintSourceEditorCmdCallback(object sender, EventArgs e)
		{
			SuspendErrorList();

			IWpfTextView view = GetActiveTextView();
			ITextDocument document;
			view.TextDataModel.DocumentBuffer.Properties.TryGetProperty(typeof(ITextDocument), out document);
			string filename = document != null ? document.FilePath : "";

			ClearErrors(filename);

			string selectionText = ActiveTextDocument.Selection.Text;

			if (!String.IsNullOrWhiteSpace(selectionText))
			{
				AnalyzeFragment(selectionText, filename, ActiveTextDocument.Selection.TopPoint.Line, ActiveTextDocument.Selection.TopPoint.DisplayColumn);
			}
			else
			{
				string lintFile = view.TextBuffer.CurrentSnapshot.GetText();
				
				AnalyzeFragment(lintFile, filename);
			}

			ResumeErrorList();
		}

		private void ErrorListCmdCallback(object sender, EventArgs e)
		{
			if (OptionsProviderRegistry.CurrentOptions.ErrorCategory.IsTaskError())
			{
				if (_errorListHelper != null)
				{
					_errorListHelper.Clear();
					ResetErrorCount();
				}
			}
		}

		private void AddSolutionOptionsFileCmdCallback(object sender, EventArgs e)
		{
			_solutionConfigurationPath = GetDefaultSolutionConfigurationPath();
			EnsureSolutionOptionsFileCreated();
			AddSolutionOptionsFileToSolutionItems();
			var newProvider = ConstructSolutionConfigurationProvider();
			OptionsProviderRegistry.PushOptionsProvider(newProvider);
			
			_hasDirtySolutionProperties = true;
			_usingSolutionOptions = true;
		}

		private FileOptionsProvider ConstructSolutionConfigurationProvider()
		{
			var currentOptions = OptionsProviderRegistry.CurrentOptions;
			var newProvider = new FileOptionsProvider("Solution", _solutionConfigurationPath);
			if (!File.Exists(_solutionConfigurationPath))
			{
				newProvider.Save(currentOptions);
			}
			return newProvider;
		}

		private const String SOLUTION_ITEMS_PROJECT_GUID = "{2150E333-8FDC-42a3-9474-1A3956D46DE8}";

		private void AddSolutionOptionsFileToSolutionItems()
		{
			_dte2.ItemOperations.AddExistingItem(_solutionConfigurationPath);
		}

		private void EnsureSolutionOptionsFileCreated()
		{
			bool solutionConfigExists = File.Exists(_solutionConfigurationPath);
			if (!solutionConfigExists)
			{
				using (FileStream fstream = File.Open(_solutionConfigurationPath, FileMode.Create))
				{
					OptionsSerializer serializer = new OptionsSerializer();
					serializer.Serialize(fstream, OptionsProviderRegistry.CurrentOptions);
					fstream.Flush();
				}
			}
		}

		private string GetDefaultSolutionConfigurationPath()
		{
			String dirName = Path.GetDirectoryName(_dte2.Solution.FullName);
			String fullyQualifiedPath = Path.Combine(dirName, JsLintOptionsFileName);
			return fullyQualifiedPath;
		}

		private void OptionsCmdCallback(object sender, EventArgs e)
		{
			OpenOptionsDialog();
		}

		private void OpenOptionsDialog()
		{
			using (OptionsForm optionsForm = new OptionsForm())
			{
				optionsForm.OptionsSourceName = _usingSolutionOptions ? "(This Solution)" : "";
				optionsForm.ShowDialog();
			}
		}

		private void buildEvents_OnBuildBegin(vsBuildScope Scope, vsBuildAction Action)
		{
			_buildScope = Scope;
			_buildAction = Action;

			if (_errorListHelper != null)
			{
				_errorListHelper.Clear();
			}
		}

		private void buildEvents_OnBuildProjConfigBegin(
			string Project,
			string ProjectConfig,
			string Platform,
			string SolutionConfig)
		{
			Options currentOptions = OptionsProviderRegistry.CurrentOptions;
			if (_buildAction == vsBuildAction.vsBuildActionClean ||
				!currentOptions.Enabled ||
				!currentOptions.RunOnBuild)
			{
				return;
			}

			var proj = _dte2.Solution.AllProjects().Single(p => p.UniqueName == Project);

			ResetErrorCount();
			SuspendErrorList();

			bool reachedTreshold;
			AnalyzeProjectItems(proj.ProjectItems, out reachedTreshold);

			ResumeErrorList();
			UpdateStatusBar(reachedTreshold);


			if (_errorCount > 0 && currentOptions.CancelBuildOnError)
			{
				WriteToErrorList("Build cancelled due to JSLint validation errors.");
				_dte2.ExecuteCommand("Build.Cancel");
			}
		}

		private TextDocument ActiveTextDocument
		{
			get
			{
				return (TextDocument)_dte2.ActiveDocument.Object("TextDocument");
			}
		}

		private IWpfTextView GetActiveTextView()
		{
			IWpfTextView view = null;
			IVsTextView vTextView = null;
			IVsTextManager txtMgr = (IVsTextManager)GetService(typeof(SVsTextManager));
			int mustHaveFocus = 1;
			txtMgr.GetActiveView(mustHaveFocus, null, out vTextView);

			IVsUserData userData = vTextView as IVsUserData;

			if (null != userData)
			{
				IWpfTextViewHost viewHost;
				object holder;
				Guid guidViewHost = DefGuidList.guidIWpfTextViewHost;
				userData.GetData(ref guidViewHost, out holder);
				viewHost = (IWpfTextViewHost)holder;
				view = viewHost.TextView;
			}
			return view;
		}

		private Array ActiveUIHierarchyItems
		{
			get
			{
				UIHierarchy h = (UIHierarchy)_dte2.ToolWindows.GetToolWindow(
					EnvDTE.Constants.vsWindowKindSolutionExplorer);
				return (Array)h.SelectedItems;
			}
		}

		private UIHierarchyItem ActiveUIHierarchyItem
		{
			get
			{
				return (UIHierarchyItem)ActiveUIHierarchyItems.GetValue(0);
			}
		}

		private IVsBuildPropertyStorage GetVsBuildPropertyStorage(Project proj)
		{
			IVsSolution sln = GetService(typeof(SVsSolution)) as IVsSolution;
			
			var activeProjs = (Array)_dte2.ActiveSolutionProjects;
			IVsHierarchy hierarchy;
			sln.GetProjectOfUniqueName(proj.FullName, out hierarchy);

			return hierarchy as IVsBuildPropertyStorage;
		}

		private const string WebsiteKind = "{E24C65DC-7377-472b-9ABA-BC803B73C61A}";
        private const string SolutionFolderKind = "{66A26720-8FB5-11D2-AA7E-00C04F688DDE}";

		private string GetCustomProperty(string name, Project proj)
		{
			if (!WebsiteKind.Equals(proj.Kind, StringComparison.Ordinal))
			{
				var storage = GetVsBuildPropertyStorage(proj);
				string value;
				storage.GetPropertyValue(
					name,
					string.Empty, (uint)_PersistStorageType.PST_PROJECT_FILE,
					out value);

				return value;
			}

			return GetWebsiteCustomProperty<string>(name, proj);
		}

		private void SetCustomProperty(string name, string value, Project proj)
		{
			if (!WebsiteKind.Equals(proj.Kind, StringComparison.Ordinal))
			{
				var storage = GetVsBuildPropertyStorage(proj);
				ErrorHandler.ThrowOnFailure(
					storage.SetPropertyValue(
						name, string.Empty, (uint)_PersistStorageType.PST_PROJECT_FILE, value));
			}
			else
			{
				SetWebsiteCustomProperty<string>(name, value, proj);
			}
		}

		private static T GetWebsiteCustomProperty<T>(string name, Project proj)
		{
			string cacheDir = VSUtilities.GetWebsiteCacheFolder(proj);
			if (cacheDir != null)
			{
				string filename = string.Concat(cacheDir, name, ".xml");
				if (File.Exists(filename))
				{
					XmlSerializer serializer = new XmlSerializer(typeof(T));
					using (FileStream fin = File.OpenRead(filename))
					{
						return (T)serializer.Deserialize(fin);
					}
				}
			}

			return default(T);
		}

		private static void SetWebsiteCustomProperty<T>(string name, T value, Project proj)
		{
			string cacheDir = VSUtilities.GetWebsiteCacheFolder(proj);
			if (cacheDir != null)
			{
				string filename = string.Concat(cacheDir, name, ".xml");
				XmlSerializer serializer = new XmlSerializer(typeof(T));
				using (FileStream fout = File.Create(filename))
				{
					serializer.Serialize(fout, value);
				}
			}
		}


        private List<string> GetSkippedNodes(Project proj)
        {
            List<string> skipped;
            if (SolutionFolderKind.Equals(proj.Kind, StringComparison.Ordinal))
            {
                skipped = new List<string>();
            }
            else
            {
                if (!_skippedNodes.TryGetValue(proj.FullName, out skipped))
                {
                    skipped = new List<string>(8);
                    _skippedNodes.Add(proj.FullName, skipped);

                    string value = GetCustomProperty("JSLintSkip", proj);
                    if (value != null)
                    {
                        skipped.AddRange(value.Split('|'));
                    }
                }
            }

            return skipped;
        }

		private static string GetRelativeName(ProjectItem projItem)
		{
            try
            {

                return GetFileName(projItem).Substring(
                    Path.GetDirectoryName(projItem.ContainingProject.FullName).Length);
            }
            catch
            {
                return "";
            }
		}

		private bool IsNodeSkipped(ProjectItem projItem, out bool skippedByFolder)
		{
			skippedByFolder = false;
			string name = GetRelativeName(projItem);
			foreach (var skipped in GetSkippedNodes(projItem.ContainingProject))
			{
				if (skipped.Length > 0 && name.StartsWith(skipped))
				{
					if (name.Length > skipped.Length)
					{
						skippedByFolder = true;
					}

					return true;
				}
			}

			return false;
		}

		private bool IsNodeSkipped(ProjectItem projItem)
		{
			bool skippedByFolder;
			return IsNodeSkipped(projItem, out skippedByFolder);
		}

		private void ToggleSkip(ProjectItem projItem)
		{
			var proj = projItem.ContainingProject;
			var skipped = GetSkippedNodes(proj);
			string name = GetRelativeName(projItem);
			if (skipped.Contains(name))
			{
				skipped.Remove(name);
			}
			else
			{
				skipped.Add(name);
			}

			SetCustomProperty("JSLintSkip", string.Join("|", skipped), proj);
		}

		private void AnalyzeProjectItems(ProjectItems projItems, out bool reachedTreshold)
		{
			var currentOptions = OptionsProviderRegistry.CurrentOptions;
			reachedTreshold = false;
			for (int i = 1; i <= projItems.Count; i++)
			{
				ProjectItem item = projItems.Item(i);
                var filename = GetFileName(item);
                IncludeFileType fileType = GetFileType(filename);

				if (fileType == IncludeFileType.Folder) // folder
				{
					AnalyzeProjectItems(item.ProjectItems, out reachedTreshold);
				}
				else if ((currentOptions.BuildFileTypes & fileType) > 0 &&
					item.FileCount == 1 &&
					!IsNodeSkipped(item))
				{
					ClearErrors(filename);
					AnalyzeFile(filename, out reachedTreshold);
				}

				if (reachedTreshold)
				{
					break;
				}
			}
		}

		private void AnalyzeFile(string filename, out bool reachedThreshold)
		{
			try
			{
				string text = File.ReadAllText(filename);
				Analyze(text, filename);
			}
			catch (Exception e)
			{
				WriteToErrorList(e.Message);
			}
			finally
			{
				reachedThreshold = _errorCount > Threshold;

				if (reachedThreshold)
				{
					WriteToErrorList(string.Format("Error threshold of {0} reached. JSLint will not generate any more errors for this operation.", Threshold));
				}
			}
		}

		private void Analyze(string fragment, string filename, int lineOffset = 1, int charOffset = 1)
		{
			if (String.IsNullOrWhiteSpace(fragment))
			{
				return;
			}

			try
			{
				int firstLineOfFragment = 0;
				IncludeFileType fileType = GetFileType(filename, fragment);

				if (fileType == IncludeFileType.CSS)
				{
					if (!fragment.StartsWith("@charset"))
					{
						if (lineOffset > 1 || OptionsProviderRegistry.CurrentOptions.FakeCSSCharset)
						{
							fragment = "@charset \"UTF-8\";" + "\n" + fragment;
							firstLineOfFragment = 1;
						}
						else
						{
							WriteError(
								"",
								lineOffset,
								1,
								"CSS Files must begin @charset to be parsed by JS Lint",
								filename);
							return;
						}
					}
				}

				IgnoreErrorSectionsHandler ignoreErrorHandler = new IgnoreErrorSectionsHandler(fragment);

				bool isJS = fileType == IncludeFileType.JS;
				var errors = _linter.Lint(fragment, OptionsProviderRegistry.CurrentOptions.JSLintOptions, isJS);
				foreach (var error in errors)
				{
					if (ignoreErrorHandler.IsErrorIgnored(error.Line, error.Column))
					{
						continue;
					}

					if (++_errorCount > Threshold)
					{
						break;
					}

					WriteError(
						error.Evidence,
						error.Line + lineOffset - (1 + firstLineOfFragment),
						error.Line == 1 + firstLineOfFragment
							? error.Column + charOffset - 1
							: error.Column,
						error.Message,
						filename,
						!isJS);
				}

				if (OptionsProviderRegistry.CurrentOptions.TODOEnabled && isJS)
				{
					var todos = TodoFinder.FindTodos(fragment);

					foreach (var error in todos)
					{
						WriteTODO(
							error.Line + lineOffset - (1 + firstLineOfFragment),
							error.Line == 1 + firstLineOfFragment
								? error.Column + charOffset - 1
								: error.Column,
							error.Message,
							filename);
					}
				}
			}
			catch (Exception e)
			{
				WriteToErrorList(e.Message);
			}
		}

		private void AnalyzeFragment(string fragment, string filename, int lineOffset = 1, int charOffset = 1)
		{
			ResetErrorCount();

			Analyze(fragment, filename, lineOffset, charOffset);

			UpdateStatusBar(_errorCount > Threshold);
		}

		private IncludeFileType GetFileType(string filename, string fragment = null)
		{
			if (filename.EndsWith(".js", StringComparison.InvariantCultureIgnoreCase))
			{
				return IncludeFileType.JS;
			}

			if (filename.EndsWith(".css", StringComparison.InvariantCultureIgnoreCase))
			{
				return IncludeFileType.CSS;
			}

			if (filename.EndsWith(".htm", StringComparison.InvariantCultureIgnoreCase) ||
				filename.EndsWith(".html", StringComparison.InvariantCultureIgnoreCase) ||
				filename.EndsWith(".aspx", StringComparison.InvariantCultureIgnoreCase) ||
				filename.EndsWith(".ascx", StringComparison.InvariantCultureIgnoreCase))
			{
				if (fragment != null && !fragment.TrimStart().StartsWith("<"))
				{
					return IncludeFileType.JS;
				}
				else
				{
					if (filename.EndsWith(".htm", StringComparison.InvariantCultureIgnoreCase) ||
						filename.EndsWith(".html", StringComparison.InvariantCultureIgnoreCase))
					{
						return IncludeFileType.HTML;
					}
				}
			}

			if (filename.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.InvariantCultureIgnoreCase))
			{
				return IncludeFileType.Folder;
			}

			return IncludeFileType.None;
		}

		private void WriteToErrorList(string message)
		{
			_errorListHelper.Write(
				Microsoft.VisualStudio.Shell.TaskCategory.BuildCompile,
				Microsoft.VisualStudio.Shell.TaskErrorCategory.Error,
				message, string.Empty, 0, 0);
		}

		private OutputWindowPane GetOutputWindowPane()
		{
			const string BuildOutputPaneGuid = "{1BD8A850-02D1-11D1-BEE7-00A0C913D1F8}";

			OutputWindow outputWindow = (_dte2.Windows.Item(EnvDTE.Constants.vsWindowKindOutput)).Object as OutputWindow;
			foreach (OutputWindowPane outputWindowPane in outputWindow.OutputWindowPanes)
			{
				if (outputWindowPane.Guid.Equals(BuildOutputPaneGuid, StringComparison.OrdinalIgnoreCase))
				{
					return outputWindowPane;
				}
			}

			return null;
		}

		private void WriteTODO(
			int line, int column, string message, string filename)
		{
			Write(OptionsProviderRegistry.CurrentOptions.TODOCategory, line, column, "TODO", message, filename);
		}


		private void WriteError(
			 string evidence, int line, int column, string message, string filename, bool forceJSLint = false)
		{
			string msgFormat;
			if (forceJSLint)
			{
				msgFormat = "JS Lint (Htm/Css): ";
			}
			else
			{
				switch (OptionsProviderRegistry.CurrentOptions.JSLintOptions.SelectedLinter)
				{
					case Linters.JSHint:
						msgFormat = "JS Hint: ";
						break;
					default:
					case Linters.JSLint:
						msgFormat = "JS Lint: ";
						break;
					case Linters.JSLintOld:
						msgFormat = "JS Lint Old: ";
						break;
				}
			}
			Write(OptionsProviderRegistry.CurrentOptions.ErrorCategory, line, column, evidence, string.Concat(msgFormat, message), filename);
		}

		private void Write(ErrorCategory category,
			int line, int column, string subcategory, string message, string filename)
		{
			if (category.IsTaskError())
			{
				_errorListHelper.Write(
					TaskCategory.BuildCompile,
					(TaskErrorCategory)category,
					message, filename, line, column);
			}
			else
			{
				TaskList taskList = (TaskList)(_dte2.Windows.Item(EnvDTE.Constants.vsWindowKindTaskList)).Object;
				taskList.TaskItems.Add(
					"JSLint", subcategory ?? string.Empty, message,
					vsTaskPriority.vsTaskPriorityHigh, null, true,
					filename, line, true, true);
			}
		}

		private void SuspendErrorList()
		{
			if (OptionsProviderRegistry.CurrentOptions.ErrorCategory.IsTaskError())
			{
				if (_errorListHelper != null)
				{
					_errorListHelper.SuspendRefresh();
				}
			}
		}

		private void ResumeErrorList(bool focus = true)
		{
			if (OptionsProviderRegistry.CurrentOptions.ErrorCategory.IsTaskError())
			{
				if (_errorListHelper != null)
				{
					_errorListHelper.ResumeRefresh(focus);
				}
			}
		}

		private void ClearErrors(string filename)
		{
			if (OptionsProviderRegistry.CurrentOptions.ErrorCategory.IsTaskError())
			{
				if (_errorListHelper != null)
				{
					_errorListHelper.ClearDocument(filename);
				}
			}
		}

		private void ResetErrorCount()
		{
			_errorCount = 0;
		}

		private void UpdateStatusBar(bool reachedTreshold)
		{
			_dte2.StatusBar.Text = string.Format("JS Lint: {0}{1} errors",
				_errorCount, reachedTreshold ? "+" : string.Empty);
		}

		#region IDisposable Members

		public void Dispose()
		{
			if (_errorListHelper != null)
			{
				_errorListHelper.Dispose();
				_errorListHelper = null;
			}

			if (_linter != null)
			{
				_linter.Dispose();
				_linter = null;
			}
		}

		#endregion

		#region IVsPersistSolutionProps Members

		public int QuerySaveSolutionProps([In] IVsHierarchy pHierarchy, [Out] VSQUERYSAVESLNPROPS[] pqsspSave)
		{
			VSQUERYSAVESLNPROPS state = VSQUERYSAVESLNPROPS.QSP_HasNoProps;
			bool isSolutionFile = (pHierarchy == null);
			if (isSolutionFile)
			{
				if (_usingSolutionOptions)
				{
					if (_hasDirtySolutionProperties)
					{
						state = VSQUERYSAVESLNPROPS.QSP_HasDirtyProps;
					}
					else
					{
						state = VSQUERYSAVESLNPROPS.QSP_HasNoDirtyProps;
					}
				}
			}
			
			pqsspSave[0] = state;

			return VSConstants.S_OK;
		}

		private const String SOLUTION_CONFIGURATION_LOCATION_PROPERTY = "SolutionConfigurationLocation";

		public int SaveSolutionProps([In] IVsHierarchy pHierarchy, [In] IVsSolutionPersistence pPersistence)
		{
			int writeToPreLoadSection = 1; //1 == true
			pPersistence.SavePackageSolutionProps(writeToPreLoadSection, pHierarchy, (IVsPersistSolutionProps)this, SolutionPropertiesKeyName);

			_hasDirtySolutionProperties = false;
			return VSConstants.S_OK;
		}

		public int ReadSolutionProps([In] IVsHierarchy pHierarchy, [In] string pszProjectName,
		[In] string pszProjectMk, [In] string pszKey, [In] int fPreLoad,
		[In] IPropertyBag pPropBag)
		{
			object solutionConfigurationLocation;
			pPropBag.Read(SOLUTION_CONFIGURATION_LOCATION_PROPERTY, out solutionConfigurationLocation,
				(IErrorLog)null, 0, null);

            _solutionConfigurationPath = Path.Combine(Path.GetDirectoryName(_dte2.Solution.FullName), (String)solutionConfigurationLocation);
            
            _usingSolutionOptions = true;

			return VSConstants.S_OK;
		}

		public int WriteSolutionProps([In] IVsHierarchy pHierarchy, [In] string pszKey, [In] IPropertyBag pPropBag)
		{
            if (_usingSolutionOptions)
            {
                string value = _solutionConfigurationPath
                        .Substring(Path.GetDirectoryName(_dte2.Solution.FullName).Length)
                        .TrimStart(DirectorySeparators);
                pPropBag.Write(SOLUTION_CONFIGURATION_LOCATION_PROPERTY, value);
            }
			return VSConstants.S_OK;
		}

		public int ReadUserOptions([In] IStream pOptionsStream, [In] string pszKey)
		{
			return VSConstants.S_OK;
		}

		public int SaveUserOptions([In] IVsSolutionPersistence pPersistence)
		{
			return VSConstants.S_OK;
		}

		public int WriteUserOptions([In] IStream pOptionsStream, [In] string pszKey)
		{
			return VSConstants.S_OK;
		}

		public int LoadUserOptions([In] IVsSolutionPersistence pPersistence, [In] uint grfLoadOpts)
		{
			return VSConstants.S_OK;
		}

		public int OnProjectLoadFailure([In] IVsHierarchy pStubHierarchy, [In] string pszProjectName, [In] string pszProjectMk,
			[In] string pszKey)
		{
			return VSConstants.S_OK;
		}

		#endregion
	}
}

