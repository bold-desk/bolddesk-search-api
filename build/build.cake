#tool "nuget:?package=xunit.runner.console"
#tool "nuget:?package=JetBrains.dotCover.CommandLineTools"
#addin "nuget:?package=Cake.Npm&version=1.0.0"
#addin nuget:?package=Cake.FileHelpers&version=4.0.1
#addin "nuget:?package=Cake.WebDeploy"
#addin "nuget:?package=Newtonsoft.Json&version=13.0.2"
#addin "nuget:?package=HtmlAgilityPack"
using System.Text.RegularExpressions

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var buildNumber = Argument("buildNumber", "");
var sourceBranch = Argument("sourceBranch", "");

var logFilename = "BoldDesk.Search.txt";
var applicationPath = @"/src/SearchApi";
var applicationSolutionPath = @"/BoldDesk.Search.Api.sln";

var sourceIncludedCodeCoverage = new List<string>(){"BoldDesk.Permission","BoldDesk.Search.Core","BoldDesk.Search.DIResolver", "BoldDesk.Search.Localization", "BoldDesk.Search.Api"};
var sourceExcludedFromCodeCoverage = new List<string>(){};

var includeFilterValue = string.Join("+:", sourceIncludedCodeCoverage.Select(i=> "*"+i+"*;"));
var excludeFilterValue = string.Join("-:", sourceExcludedFromCodeCoverage.Select(i=> "*"+i+"*;"));
var codeCoverageFilter = string.Format("+:{0}-:{1}", includeFilterValue, excludeFilterValue);

var unitTestProjectDirPathPattern = @"../test/UnitTests/*.csproj";
var integrationTestProjectDirPathPattern = @"../test/IntegrationTests/*.csproj";

var projectFramework = "net6.0";

var cireports = Argument("cireports", "../cireports");
var SCSReportDir = cireports + "/securitycodescan";
var FXReportDir = cireports + "/fxcopviolation";
var StyleCopReportsDir = cireports + "/stylecopviolation";
var xUnitViolationReportDir = cireports + "/xunitviolation";
var xUnitViolationReportDirForIntegrationHook = cireports + "/xunitviolation";
var codecoverageReportDir = cireports + "/codecoverage/UnitTesting";
var codecoverageReportDirForIntegrationTesting = cireports + "/codecoverage/IntegrationTesting";
var xunitReportDir = cireports + "/xunitreport/UnitTesting";
var xunitReportDirForIntegrationHook = cireports + "/xunitreport/UnitTesting_IntegrationHook";
var xunitReportDirForIntegrationTesting = cireports + "/xunitreport/IntegrationTesting";
var DetectSecretScanReportDir = cireports + "/secret";

var styleCopReport = StyleCopReportsDir + "/StyleCopViolations.txt";
var fxCopReport = FXReportDir + "/FXCopViolations.txt";
var securityCodeScanReport = SCSReportDir + "/SecurityCodeScanViolations.txt";
var xunitReport = xUnitViolationReportDir + "/xUnitViolations.txt";
var dotCodeCoverageReport = codecoverageReportDir + "/UnitTestCover.dcvr";
var dotCodeCoverageHTMLReport = codecoverageReportDir + "/UnitTestCover.html";
var dotCodeCoverageXMLReport = codecoverageReportDir + "/UnitTestCover.xml";
var dotCodeCoverageReportForIntegrationTesting = codecoverageReportDirForIntegrationTesting + "/IntegrationTestCover.dcvr";
var dotCodeCoverageHTMLReportForIntegrationTesting = codecoverageReportDirForIntegrationTesting + "/IntegrationTestCover.html";
var dotCodeCoverageXMLReportForIntegrationTesting = codecoverageReportDirForIntegrationTesting + "/IntegrationTestCover.xml";
var unitTestingReport = xunitReportDir + "/TestResult.xml";
var unitTestingReportForIntegrationHook = xunitReportDirForIntegrationHook + "/TestResult.xml";
var integrationTestingReport = xunitReportDirForIntegrationTesting + "/TestResult.xml";
var testingHTMLReport = "TestResult.html";

var buildStatus = true;

var errorlogFolder = cireports + "/errorlogs/";
var waringsFolder = cireports + "/warnings/";

var apiServerIP = Argument<string>("apiServerIP","");
var apiServerPort = Argument<string>("apiServerPort","");
var apiSiteName = Argument<string>("apiSiteName","");
var apiServerUserName = Argument<string>("apiServerUserName","");
var apiServerPassword = Argument<string>("apiServerPassword","");
 
var currentDirectory = MakeAbsolute(Directory("../"));
var currentDirectoryInfo = new DirectoryInfo(currentDirectory.FullPath);

var projDir =  currentDirectory + applicationPath;
var binDir = String.Concat(projDir,"bin" ) ;

var solutionFile = currentDirectory + applicationSolutionPath; 

var outputDir = Directory(binDir) + Directory(configuration);
var fxCopViolationCount = 0;
var styleCopViolationCount = 0;
var securityCodeScanWarningCount = 0;
var xUnitWarningCount = 0;

var securityCodeRegex = "warning SCS";
var fxCopRegex = "warning CA";
var styleCopRegex = "warning SA";
var styleCopAnalyzersRegex = "warning SX";
var xUnitRegex = "warning xUnit";
var apiAnalyzerRegex = "warning API";
var asyncAnalyzerRegex = "warning AsyncFixer";
var cSharpAnalyzerRegex = "warning RS";
var mvcAnalyzerRegex = "warning MVC";
var entityFrameworkRegex = "warning EF";
var rosylnatorAnalyzerRegex = "warning RCS";

var startTime = "";
var endTime = "";

var framework = Argument("framework", projectFramework);

var buildSettings = new DotNetCoreBuildSettings
     {
         Framework = framework,
         Configuration = configuration,
         OutputDirectory = outputDir
     };

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
	{
	try {
		var binDirectories = currentDirectoryInfo.GetDirectories("bin", SearchOption.AllDirectories);
		var objDirectories = currentDirectoryInfo.GetDirectories("obj", SearchOption.AllDirectories);
		
		foreach(var directory in binDirectories){
			CleanDirectories(directory.FullName);
		}
		
		foreach(var directory in objDirectories){
			CleanDirectories(directory.FullName);
		}

		if (DirectoryExists(outputDir))
		{
			DeleteDirectory(outputDir, recursive:true);
		}
	}
	catch(Exception ex) {
		throw new Exception(String.Format("Please fix the clean task failures"));  
	}
});

Task("Restore")
    .Does(() => {
        DotNetCoreRestore(solutionFile);
    });
	
Task("DeleteLogFile")
	.Does(()=>{
		try {		
			if(FileExists(errorlogFolder + logFilename)){
				DeleteFile(errorlogFolder + logFilename);
			}
			
			if(FileExists(waringsFolder + logFilename)){
				DeleteFile(waringsFolder + logFilename);
			}	
		}
		catch(Exception ex) {
			throw new Exception(String.Format("Please fix the delete log file task failures"));  
		}		
	});
	
	var npmSettings = 
        new NpmInstallSettings 
        {
            WorkingDirectory = projDir 			     
        }; 

Task("Build")   
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .Does(() => {	
	try { 	
  
	 // NpmInstall(npmSettings); 

	  MSBuild(solutionFile , settings => 
	   settings.SetConfiguration(configuration)
	   .WithProperty("DeployOnBuild","true")
       .AddFileLogger(new MSBuildFileLogger{LogFile = waringsFolder + logFilename, MSBuildFileLoggerOutput=MSBuildFileLoggerOutput.WarningsOnly})
	  );  
	   } 	
	catch(Exception ex) {        
		throw new Exception(String.Format("Please fix the project compilation failures"));  
	}
    }); 

Task("Get-Security-Scan-Reports")
 .Does(() =>
 {
	 try {
		var securityCodeScanWarning = FileReadText(waringsFolder + logFilename);    
		securityCodeScanWarningCount = Regex.Matches(securityCodeScanWarning, securityCodeRegex).Count;

		if (DirectoryExists(SCSReportDir))
		{
		DeleteDirectory(SCSReportDir, recursive:true);
		}

		if(securityCodeScanWarningCount != 0)
		{        
		Information("There are {0} Security Code violations found", securityCodeScanWarningCount);
		}
		
		if (!DirectoryExists(SCSReportDir)) {
			CreateDirectory(SCSReportDir);
		}

		FileWriteText(securityCodeScanReport, "Security Violations Error(s) : " + securityCodeScanWarningCount);
	}
	catch(Exception ex) {
		throw new Exception(String.Format("Please fix the Get Security Scan Reports task failures"));  
	}
});

Task("Get-Fx-cop-Reports")
 .Does(() =>
 { 
	try {
		if (DirectoryExists(FXReportDir))
		{
		DeleteDirectory(FXReportDir, recursive:true);
		}	 

		var fxCopWarning = FileReadText(waringsFolder + logFilename);
		fxCopViolationCount = Regex.Matches(fxCopWarning, fxCopRegex).Count;
		fxCopViolationCount += Regex.Matches(fxCopWarning, apiAnalyzerRegex).Count;
		fxCopViolationCount += Regex.Matches(fxCopWarning, asyncAnalyzerRegex).Count;
		fxCopViolationCount += Regex.Matches(fxCopWarning, cSharpAnalyzerRegex).Count;
		fxCopViolationCount += Regex.Matches(fxCopWarning, mvcAnalyzerRegex).Count;
		fxCopViolationCount += Regex.Matches(fxCopWarning, entityFrameworkRegex).Count; 
		fxCopViolationCount += Regex.Matches(fxCopWarning, rosylnatorAnalyzerRegex).Count; 

		if(fxCopViolationCount != 0)
		{        
		Information("There are {0} FXCop violations found", fxCopViolationCount);
		}
		
		if (!DirectoryExists(FXReportDir)) {
			CreateDirectory(FXReportDir);
		}
		
		FileWriteText(fxCopReport, "FXCop Error(s) : " + fxCopViolationCount);
	}
	catch(Exception ex) {
		throw new Exception(String.Format("Please fix the Get FxCop Reports task failures"));  
	}
});

Task("Get-StyleCop-Reports")
 .Does(() =>
 {
	try {
		if (DirectoryExists(StyleCopReportsDir))
		{
		DeleteDirectory(StyleCopReportsDir, recursive:true);
		}	

		var styleCopWarning = FileReadText(waringsFolder + logFilename);
		styleCopViolationCount += Regex.Matches(styleCopWarning, styleCopRegex).Count;
		styleCopViolationCount += Regex.Matches(styleCopWarning, styleCopAnalyzersRegex).Count;

		if(styleCopViolationCount != 0)
		{        
		Information("There are {0} StyleCop violations found", styleCopViolationCount);
		}
		
		if (!DirectoryExists(StyleCopReportsDir)) {
			CreateDirectory(StyleCopReportsDir);
		}

		FileWriteText(styleCopReport, "Style Cop Error(s) : " + styleCopViolationCount);
	}
	catch(Exception ex) {
		throw new Exception(String.Format("Please fix the Get StyleCop Reports task failures"));  
	}
});

Task("Get-xUnit-Reports")
 .Does(() =>
 { 
	try {
		if (DirectoryExists(xUnitViolationReportDir))
		{
		DeleteDirectory(xUnitViolationReportDir, recursive:true);
		}	

		var xUnitWarning = FileReadText(waringsFolder + logFilename);
		xUnitWarningCount += Regex.Matches(xUnitWarning, xUnitRegex).Count;

		if(xUnitWarningCount != 0)
		{        
		Information("There are {0} xUnit violations found", xUnitWarningCount);
		}
		
		if (!DirectoryExists(xUnitViolationReportDir)) {
			CreateDirectory(xUnitViolationReportDir);
		}
		
		FileWriteText(xunitReport, "xUnit Violations Error(s) : " + xUnitWarningCount);
	}
	catch(Exception ex) {
		throw new Exception(String.Format("Please fix the Get xUnit Reports task failures"));
	}
});

Task("Code-Coverage")
    .ContinueOnError()
    .Does(() =>
    {
		if (!DirectoryExists(cireports))
		{
			CreateDirectory(cireports);
			CreateDirectory(xunitReportDir);
		} else {
			if(!DirectoryExists(xunitReportDir))
			{
				CreateDirectory(xunitReportDir);
			}
			else {
				DeleteDirectory(xunitReportDir, recursive:true);
				CreateDirectory(xunitReportDir);
			}
		}

		if (!DirectoryExists(codecoverageReportDir))
		{
			CreateDirectory(codecoverageReportDir); 
		} else { 
				DeleteDirectory(codecoverageReportDir, recursive:true);
				CreateDirectory(codecoverageReportDir);	
		}

		var projects = GetFiles(unitTestProjectDirPathPattern);
		var settings = new DotCoverCoverSettings()
                    .WithFilter(codeCoverageFilter)
					.WithAttributeFilter("System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute");

        foreach(var project in projects)
        {			
            DotCoverCover(
                x => x.DotNetCoreTest(
                     project.FullPath,
                     new DotNetCoreTestSettings() {
						  Configuration = configuration,
						  Logger = $"html;LogFileName={testingHTMLReport}",
        				  NoBuild = true,
						  ResultsDirectory = new DirectoryPath(xunitReportDir)
						  }
                ),
                dotCodeCoverageReport,
				settings
            );
        }
    });

Task("Unit-Testing")
    .IsDependentOn("Code-Coverage")
    .ContinueOnError()
    .Does(() =>
    {
		startTime = DateTime.Now.ToString("dd MMM, yyyy HH:mm:ss"); // Setting Start time

		try {
			DotCoverReport(dotCodeCoverageReport, dotCodeCoverageHTMLReport,
				new DotCoverReportSettings {
					ReportType = DotCoverReportType.HTML
			});			

			DotCoverReport(dotCodeCoverageReport, dotCodeCoverageXMLReport,
				new DotCoverReportSettings {
					ReportType = DotCoverReportType.XML
			});
			
			var  coveragePercent =(from elements in System.Xml.Linq.XDocument.Load(dotCodeCoverageXMLReport).Descendants("Root") 
							select (string)elements.Attribute("CoveragePercent")).FirstOrDefault();
			
			FileStream fs = new FileStream(codecoverageReportDir+"/UnitTestCover.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite);
			StreamWriter writer = new StreamWriter(fs);
			writer.Write(coveragePercent);
			writer.Close();
			
			Information("CoveragePercent : "+coveragePercent);
		}
		catch(Exception ex) {
			throw new Exception(String.Format("Please fix the Unit testing task failures"));  
		}

		endTime = DateTime.Now.ToString("dd MMM, yyyy HH:mm:ss"); // Setting EndTime
		string timeLog = $"{startTime} - {endTime}";
		FileWriteText("time.txt", timeLog);
    });
	
Task("Code-Coverage-By-Integration-Testing")
    .ContinueOnError()
    .Does(() =>
    {
		if (!DirectoryExists(cireports))
		{
			CreateDirectory(cireports);
			CreateDirectory(xunitReportDirForIntegrationTesting);
		} else {
			if(!DirectoryExists(xunitReportDirForIntegrationTesting))
			{
				CreateDirectory(xunitReportDirForIntegrationTesting);
			}
			else {
				DeleteDirectory(xunitReportDirForIntegrationTesting, recursive:true);
				CreateDirectory(xunitReportDirForIntegrationTesting);
			}
		}

		if (!DirectoryExists(codecoverageReportDirForIntegrationTesting))
		{
			CreateDirectory(codecoverageReportDirForIntegrationTesting); 
		} else { 
				DeleteDirectory(codecoverageReportDirForIntegrationTesting, recursive:true);
				CreateDirectory(codecoverageReportDirForIntegrationTesting);	
		}

		var projects = GetFiles(integrationTestProjectDirPathPattern);
		var settings = new DotCoverCoverSettings()
                    .WithFilter(codeCoverageFilter)
					.WithAttributeFilter("System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute");

        foreach(var project in projects)
        {
            DotCoverCover(
                x => x.DotNetCoreTest(
                     project.FullPath,
                     new DotNetCoreTestSettings() {
						  Configuration = configuration,
						  Logger = $"html;LogFileName={testingHTMLReport}",
        				  NoBuild = true,
						  ResultsDirectory = new DirectoryPath(xunitReportDirForIntegrationTesting)
						}
                ),
                dotCodeCoverageReportForIntegrationTesting,
				settings
            );
        }
    });
	
Task("Integration-Testing")
    .IsDependentOn("Code-Coverage-By-Integration-Testing")
    .ContinueOnError()
    .Does(() =>
    {
		DotCoverReport(dotCodeCoverageReportForIntegrationTesting, dotCodeCoverageHTMLReportForIntegrationTesting,
			new DotCoverReportSettings {
				ReportType = DotCoverReportType.HTML
		});			

		DotCoverReport(dotCodeCoverageReportForIntegrationTesting, dotCodeCoverageXMLReportForIntegrationTesting,
			new DotCoverReportSettings {
				ReportType = DotCoverReportType.XML
		});
		
		var  coveragePercent =(from elements in System.Xml.Linq.XDocument.Load(dotCodeCoverageXMLReportForIntegrationTesting).Descendants("Root") 
						select (string)elements.Attribute("CoveragePercent")).FirstOrDefault();
		
		FileStream fs = new FileStream(codecoverageReportDirForIntegrationTesting+"/IntegrationTestCover.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite);
		StreamWriter writer = new StreamWriter(fs);
		writer.Write(coveragePercent);
		writer.Close();
		
		Information("CoveragePercent : "+coveragePercent);
    });
	
Task("codeviolation")
    .IsDependentOn("Get-StyleCop-Reports")
	.IsDependentOn("Get-Fx-cop-Reports")
	.IsDependentOn("Get-Security-Scan-Reports")
	.IsDependentOn("Get-xUnit-Reports")
    .IsDependentOn("Unit-Testing")
	.IsDependentOn("Integration-Testing")
	.IsDependentOn("GitLeaks")
    .Does(() =>
{
});

Task("Download-GitLeaks")
  .WithCriteria( !FileExists("./tools/GitLeaks-exe.zip"))
  .ContinueOnError()
  .Does(() =>
{
 
    DownloadFile("https://github.com/zricethezav/gitleaks/releases/download/v8.15.2/gitleaks_8.15.2_windows_x64.zip", "./tools/GitLeaks-exe.zip");
	Unzip("./tools/GitLeaks-exe.zip", "./tools/GitLeaks/");
     
});

Task("GitLeaks")
  .IsDependentOn("Download-GitLeaks")
  .Does(() =>
{	
	try
	{
		if(DirectoryExists(DetectSecretScanReportDir))
		{
			DeleteDirectory(DetectSecretScanReportDir, recursive:true);
		}
		if(!DirectoryExists(cireports))
		{
			CreateDirectory(cireports);
		}
		if(!DirectoryExists(DetectSecretScanReportDir))
		{
			CreateDirectory(DetectSecretScanReportDir);
		}

		//Download Gitleaks if not exists
		if (!FileExists("./tools/GitLeaks/gitleaks.exe"))
		{	
			DownloadFile("https://github.com/zricethezav/gitleaks/releases/download/v8.15.2/gitleaks_8.15.2_windows_x64.zip", "./tools/GitLeaks-exe.zip");
			Unzip("./tools/GitLeaks-exe.zip", "./tools/GitLeaks/"); 
		}

		//Scan for secrets and export report to GitLeaksReport.json
		StartProcess("./tools/GitLeaks/gitleaks.exe", new ProcessSettings{ Arguments ="detect --no-git --report-path "+DetectSecretScanReportDir+"/GitLeaksReport.json --source ../ --config gitleaks.toml"});

		var jsonString = FileReadText(DetectSecretScanReportDir+"/GitLeaksReport.json");
		var jsonObject = Newtonsoft.Json.Linq.JArray.Parse(jsonString);
		var count = jsonObject.Count;
		
		Information("Number of objects in the JSON file: {0}", count);
	}
	catch(Exception ex)
	{
		throw new Exception(String.Format("Exception thrown in secret detection process, please fix this: " + ex));  
	}
});

// Downloading Syncfusion-update-testresult-exe from amazonaws
Task("Syncfusion-update-testresult-exe")
	.WithCriteria( !FileExists("./Syncfusion.UpdateTestResults_Exe.zip"))
	.ContinueOnError()
	.Does(() =>
{	
	Information("Downloading the Syncfusion.UpdateTestResults_Exe.zip");
	DownloadFile("https://s3.amazonaws.com/files2.syncfusion.com/Installs/TestResultsUpdate/Syncfusion.UpdateTestResults_Exe.zip", "./Syncfusion.UpdateTestResults_Exe.zip");
	Information("UnZipping the file");
	Unzip("./Syncfusion.UpdateTestResults_Exe.zip", "./UpdateResults/"); 
	Information("File UnZipped");
	var sourceDirectory = "./UpdateResults"; 
    var targetDirectory = "./";
	if (System.IO.Directory.Exists(sourceDirectory) && System.IO.Directory.Exists(targetDirectory))
    {
        var filesToMove = System.IO.Directory.GetFiles(sourceDirectory);
        foreach (var filePath in filesToMove)
        {
            var fileName = System.IO.Path.GetFileName(filePath);
            var targetFilePath = System.IO.Path.Combine(targetDirectory, fileName);
			if (System.IO.File.Exists(targetFilePath))
            {
                Error($"File {fileName} already exists in the target directory.");
                continue;
            }
           try
            {
                System.IO.File.Move(filePath, targetFilePath);
                Information($"Moved: {fileName}");
            }
            catch (System.IO.IOException)
            {
                Error($"Could not move {fileName}. It might be in use or locked.");
            }
        }
        Information("All files moved successfully.");
    }
    else
    {
        Error("Source or target directory does not exist.");
    }
});

// Executing configurersa bat file
Task("Execute-bat-file")
	.ContinueOnError()
	.Does(() => 
{
	Information("Executing configurersa.bat");
	//Running configurersa bat file
	StartProcess("configurersa.bat");
	Information("Executed successfully");
});

// To send test result updates to centralized DB
Task("Send-result-updates")
	.ContinueOnError()
	.IsDependentOn("Syncfusion-update-testresult-exe")
	.IsDependentOn("Execute-bat-file")
	.Does(() =>
{
	//Get user info - emailid
	StartProcess("powershell.exe", new ProcessSettings { Arguments = "git show -s --pretty=%ae > useremailinfo.txt" });
	var userEmailId = System.IO.File.ReadAllText("useremailinfo.txt").Trim();
	// For kb-api unit test results
	var htmlfilepath = xunitReportDir +"/"+ "TestResult.html"; 
	var htmlContent = System.IO.File.ReadAllText(htmlfilepath); // Reading HTML file from path
	// Load the HTML content using HTML Agility Pack
	var htmlDoc = new HtmlAgilityPack.HtmlDocument();
	htmlDoc.LoadHtml(htmlContent); // Loading the HTML File
	var totaltests = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='total-tests']").InnerText;
	var passedTests = htmlDoc.DocumentNode.SelectSingleNode("//span[@class='passedTests']").InnerText;
	var failedTests = htmlDoc.DocumentNode.SelectSingleNode("//span[@class='failedTests']").InnerText;
	var skippedTests = htmlDoc.DocumentNode.SelectSingleNode("//span[@class='skippedTests']").InnerText;
	var SuccessStatus = "";
	var resultLocation = xunitReportDir + "/" + testingHTMLReport;
	var TestRunLink = $"https://jenkins.syncfusion.com/job/BoldDesk/job/bolddesk-kb-api/{buildNumber}/artifact/cireports/xunitreport/UnitTesting/TestResult.html";
	if(totaltests == passedTests)
	{
		SuccessStatus = "Success";
	}
	else
	{
		SuccessStatus = "Failure";
	}
	
	string startEndTime = FileReadText("time.txt");
	string[] times = startEndTime.Split('-');
	
	if (times.Length == 2)
	{
		startTime = times[0].Trim();
		endTime = times[1].Trim();
	}
	else
	{
		Information("Invalid start and end time format in text file.");
	}
	
	// Executing the command to send data to Centralized DB
	StartProcess("Syncfusion.UpdateTestResults.exe", new ProcessSettings
		 		{
		 			Arguments = "/Platform:\"BoldDesk\" /Project:\"search-api\" /Control:\"MainApp\" /Tags:\"\" /TestingTool:\"xUnit\" /StartTime:\""+startTime+"\" /EndTime:\""+endTime+"\" /Status:\""+SuccessStatus+"\" /TotalTestCase:"+totaltests+" /SuccessCount:"+passedTests+" /FailureCount:"+failedTests+" /NotRunCount:"+skippedTests+" /TestRunName:\"kb-api-unit-test\" /TestRunLink:\""+TestRunLink+"\" /Branch:\""+sourceBranch+"\" /Version:\"\" /UpdatedBy:\""+userEmailId+"\""
		 		}
		 	 );	
			 
	// Moving log file to a location
	Information("Moving log file");
	var sourceDirectory = "./"; // Current directory
    var destinationDirectory = xunitReportDir; // Replace with the destination directory
    var logFiles = GetFiles(sourceDirectory + "**/*.log");
    foreach (var logFile in logFiles)
    {
        var destinationPath = new DirectoryPath(destinationDirectory)
            .CombineWithFilePath(logFile.GetFilename());
        Information($"Copying {logFile.GetFilename()} to {destinationPath}");
        CopyFile(logFile, destinationPath);
    }
	Information("Log files moved");
	
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Build")
    .IsDependentOn("Codeviolation")
	.IsDependentOn("GitLeaks");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);