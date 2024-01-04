#tool "nuget:?package=xunit.runner.console"
#tool "nuget:?package=JetBrains.dotCover.CommandLineTools"
#addin nuget:?package=Cake.FileHelpers&version=4.0.1
#addin "nuget:?package=Newtonsoft.Json&version=13.0.2"
#addin "nuget:?package=HtmlAgilityPack"
using System.Text.RegularExpressions;

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
var sourceExcludedFromCodeCoverage = new List<string>(){"BoldDesk.Search.Api.UnitTests"};

var includeFilterValue = string.Join("+:", sourceIncludedCodeCoverage.Select(i=> "*"+i+"*;"));
var excludeFilterValue = string.Join("-:", sourceExcludedFromCodeCoverage.Select(i=> "*"+i+"*;"));
var codeCoverageFilter = string.Format("+:{0}-:{1}", includeFilterValue, excludeFilterValue);

var unitTestProjectDirPathPattern = @"../test/UnitTests/*.csproj";
var integrationTestProjectDirPathPattern = @"../test/IntegrationTests/*.csproj";

var cireports = Argument("cireports", "../cireports");
var SCSReportDir = cireports + "/securitycodescan";
var FXReportDir = cireports + "/fxcopviolation";
var StyleCopReportsDir = cireports + "/stylecopviolation";
var DetectSecretScanReportDir = cireports + "/secret";


var xUnitViolationReportDir = cireports + "/xunitviolation";
var xunitReportDir = cireports + "/xunitreport/UnitTesting";
var codecoverageReportDir = cireports + "/codecoverage/UnitTestCover";

var styleCopReport = StyleCopReportsDir + "/StyleCopViolations.txt";
var styleCopXMLReport = StyleCopReportsDir + "/StyleCopViolations.xml";

var fxCopReport = FXReportDir + "/FXCopViolations.txt";
var fxCopXMLReport = FXReportDir + "/FXCopViolations.xml";

var securityCodeScanReport = SCSReportDir + "/SecurityCodeScanViolations.txt";
var securityCodeScanXMLReport = SCSReportDir + "/SecurityCodeScanViolations.xml";

var xunitReport = xUnitViolationReportDir + "/xUnitViolations.txt";
var xunitXMLReport = xUnitViolationReportDir + "/xUnitViolations.xml";

var dotCodeCoverageReport = codecoverageReportDir+"/UnitTestCover.dcvr";
var dotCodeCoverageHTMLReport = codecoverageReportDir+"/UnitTestCover.html";
var dotCodeCoverageXMLReport = codecoverageReportDir+"/UnitTestCover.xml";
var unitTestingReport = xunitReportDir+"/TestResult.xml";

var testingHTMLReport = "TestResult.html";
var buildStatus = true;

var errorlogFolder = cireports + "/errorlogs/";
var warningsFolder = cireports + "/warnings/";

var apiServerIP=Argument<string>("apiServerIP","");
var apiServerPort=Argument<string>("apiServerPort","");
var apiSiteName=Argument<string>("apiSiteName","");
var apiServerUserName=Argument<string>("apiServerUserName","");
var apiServerPassword=Argument<string>("apiServerPassword","");
 
var currentDirectory=MakeAbsolute(Directory("../"));
var currentDirectoryInfo=new DirectoryInfo(currentDirectory.FullPath);

var projDir =  currentDirectory + applicationPath;
var binDir = String.Concat(projDir,"bin" ) ;

var solutionFile = currentDirectory + applicationSolutionPath; 

var outputDir = Directory(binDir) + Directory(configuration);
var fxCopViolationCount = 0;
var styleCopViolationCount = 0;
var securityCodeScanWarningCount = 0;
var xUnitWarningCount = 0;

string securityCodeRegexPattern = @"(.*warning SCS\d+: .+)";
string fxCopRegexPattern = @"(.*warning (CA|CS|API|AsyncFixer|RS|MVC|EF|RCS|MA)\d+: .+)";
string styleCopRegexPattern = @"(.*warning (SA|SX)\d+: .+)";
string xUnitRegexPattern = @"(.*warning xUnit\d+: .+)";

var violationsReportRootXmlFormat = "<Violations>{0}</Violations>";
var violationsReportInnerXmlFormat = "<Violation Source=\"{0}\"></Violation>" + Environment.NewLine;

var startTime = "";
var endTime = "";

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
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
            DeleteDirectory(outputDir, new DeleteDirectorySettings { Recursive = true });
        }
});

Task("Restore")
    .Does(() => {
        DotNetRestore(solutionFile);
    });
	
Task("DeleteLogFile")
	.Does(()=>{
		
		if(FileExists(errorlogFolder + logFilename)){
			DeleteFile(errorlogFolder + logFilename);
		}
		
		if(FileExists(warningsFolder + logFilename)){
			DeleteFile(warningsFolder + logFilename);
		}				
	});

Task("Build")   
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .Does(() => {	
	try {

		CreateDirectory(warningsFolder);
		CreateDirectory(errorlogFolder); 	
  
	 	// NpmInstall(npmSettings); 

	  	var buildSettings = new DotNetBuildSettings
		{
			Configuration = configuration,
			NoRestore = true,
			MSBuildSettings = new DotNetMSBuildSettings().AddFileLogger(new MSBuildFileLoggerSettings{LogFile = warningsFolder + logFilename, SummaryOutputLevel = MSBuildLoggerOutputLevel.WarningsOnly})
		};
			
			DotNetBuild(solutionFile, buildSettings);   
	} 	
	catch(Exception ex) {        
		throw new Exception(String.Format("Please fix the project compilation failures"));  
	}
    }); 

Task("Get-Security-Scan-Reports")
 .Does(() =>
 { 
    var securityCodeScanWarning = FileReadText(warningsFolder + logFilename);    
	var securityCodeScanWarningLogs = Regex.Matches(securityCodeScanWarning, securityCodeRegexPattern);
	securityCodeScanWarningCount = securityCodeScanWarningLogs.Count;


	if (DirectoryExists(SCSReportDir))
    {
	 DeleteDirectory(SCSReportDir, new DeleteDirectorySettings { Recursive = true });
	}

    if(securityCodeScanWarningCount != 0)
    {        
       Information("There are {0} Security Code violations found", securityCodeScanWarningCount);
    }
	
	if (!DirectoryExists(SCSReportDir)) {
		CreateDirectory(SCSReportDir);
	}

	FileWriteText(securityCodeScanReport, "Security Violations Error(s) : " + securityCodeScanWarningCount);
	var reportInnerXmlValue = string.Join("", securityCodeScanWarningLogs.Select(i => string.Format(violationsReportInnerXmlFormat, i.Value.Trim())));
	FileWriteText(securityCodeScanXMLReport, string.Format(violationsReportRootXmlFormat, reportInnerXmlValue));
});

Task("Get-Fx-cop-Reports")
 .Does(() =>
 { 
	if (DirectoryExists(FXReportDir))
    {
	 DeleteDirectory(FXReportDir, new DeleteDirectorySettings { Recursive = true });
	}	 

	var fxCopWarning = FileReadText(warningsFolder + logFilename);
	var fxCopWarningLogs = Regex.Matches(fxCopWarning, fxCopRegexPattern);
	fxCopViolationCount = fxCopWarningLogs.Count;

    if(fxCopViolationCount != 0)
    {        
       Information("There are {0} FXCop violations found", fxCopViolationCount);
    }
	
	if (!DirectoryExists(FXReportDir)) {
		CreateDirectory(FXReportDir);
	}
	
	FileWriteText(fxCopReport, "FXCop Error(s) : " + fxCopViolationCount);
	var reportInnerXmlValue = string.Join("", fxCopWarningLogs.Select(i => string.Format(violationsReportInnerXmlFormat, i.Value.Trim())));
	FileWriteText(fxCopXMLReport, string.Format(violationsReportRootXmlFormat, reportInnerXmlValue));
});

Task("Get-StyleCop-Reports")
 .Does(() =>
 {
	var styleCopWarning = FileReadText(warningsFolder + logFilename);
	var styleCopWarningLogs = Regex.Matches(styleCopWarning, styleCopRegexPattern);
	styleCopViolationCount = styleCopWarningLogs.Count;
		
	if (DirectoryExists(StyleCopReportsDir)){
		DeleteDirectory(StyleCopReportsDir, new DeleteDirectorySettings { Recursive = true });
	}	

    if(styleCopViolationCount != 0)
    {        
       Information("There are {0} StyleCop violations found", styleCopViolationCount);
    }
	
	if (!DirectoryExists(StyleCopReportsDir)) {
		CreateDirectory(StyleCopReportsDir);
	}

	FileWriteText(styleCopReport, "Style Cop Error(s) : " + styleCopViolationCount);
	var reportInnerXmlValue = string.Join("", styleCopWarningLogs.Select(i => string.Format(violationsReportInnerXmlFormat, i.Value.Trim())));
	FileWriteText(styleCopXMLReport, string.Format(violationsReportRootXmlFormat, reportInnerXmlValue));
});

Task("Get-xUnit-Reports")
 .Does(() =>
 { 
	var xUnitWarning = FileReadText(warningsFolder + logFilename);
	var xUnitWarningLogs = Regex.Matches(xUnitWarning, xUnitRegexPattern);
	xUnitWarningCount = xUnitWarningLogs.Count;
	if (DirectoryExists(xUnitViolationReportDir)){
		DeleteDirectory(xUnitViolationReportDir, new DeleteDirectorySettings { Recursive = true });
		}

    if(xUnitWarningCount != 0)
    {        
       Information("There are {0} xUnit violations found", xUnitWarningCount);
    }
	
	if (!DirectoryExists(xUnitViolationReportDir)) {
		CreateDirectory(xUnitViolationReportDir);
	}
	
	FileWriteText(xunitReport, "xUnit Violations Error(s) : " + xUnitWarningCount);
	var reportInnerXmlValue = string.Join("", xUnitWarningLogs.Select(i => string.Format(violationsReportInnerXmlFormat, i.Value.Trim())));
	FileWriteText(xunitXMLReport, string.Format(violationsReportRootXmlFormat, reportInnerXmlValue));
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
				DeleteDirectory(xunitReportDir, new DeleteDirectorySettings { Recursive = true });
				CreateDirectory(xunitReportDir);
			}
		}

		if (!DirectoryExists(codecoverageReportDir))
		{
			CreateDirectory(codecoverageReportDir); 
		} else { 
				DeleteDirectory(codecoverageReportDir, new DeleteDirectorySettings { Recursive = true });
				CreateDirectory(codecoverageReportDir);	
		}

		var projects = GetFiles(unitTestProjectDirPathPattern);
		var settings = new DotCoverCoverSettings()
                    .WithFilter(codeCoverageFilter)
					.WithAttributeFilter("System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute");

        foreach(var project in projects)
        {
            DotCoverCover(
                x => x.DotNetTest(
                     project.FullPath,
						new DotNetTestSettings() 
						{
							Configuration = configuration,
							Loggers = new [] { $"html;LogFileName={testingHTMLReport}" },
							NoBuild = true,
							NoRestore = true,
							ResultsDirectory = new DirectoryPath(xunitReportDir)
						}),

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

		endTime = DateTime.Now.ToString("dd MMM, yyyy HH:mm:ss"); // Setting End time
		string timeLog = $"{startTime} - {endTime}";
        FileWriteText("time.txt", timeLog);
    });

Task("codeviolation")
    .IsDependentOn("Get-StyleCop-Reports")
	.IsDependentOn("Get-Fx-cop-Reports")
	.IsDependentOn("Get-Security-Scan-Reports")
	.IsDependentOn("Get-xUnit-Reports")
    .IsDependentOn("Unit-Testing")
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
			DeleteDirectory(DetectSecretScanReportDir, new DeleteDirectorySettings { Recursive = true });
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
	var TestRunLink = $"https://jenkins.syncfusion.com/job/BoldDesk/job/bolddesk-search-api/{buildNumber}/artifact/cireports/xunitreport/UnitTesting/TestResult.html";
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
		 			Arguments = "/Platform:\"BoldDesk\" /Project:\"search-api\" /Control:\"MainApp\" /Tags:\"\" /TestingTool:\"xUnit\" /StartTime:\""+startTime+"\" /EndTime:\""+endTime+"\" /Status:\""+SuccessStatus+"\" /TotalTestCase:"+totaltests+" /SuccessCount:"+passedTests+" /FailureCount:"+failedTests+" /NotRunCount:"+skippedTests+" /TestRunName:\"unit-test\" /TestRunLink:\""+TestRunLink+"\" /Branch:\""+sourceBranch+"\" /Version:\"\" /UpdatedBy:\""+userEmailId+"\""
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