node ('BoldDesk')
{
timestamps
{
  timeout(time: 2700000, unit: 'MILLISECONDS')
  {
    stage 'Checkout'
    try
    {
		checkout scm
    }
    catch(Exception e)
    {
        echo "Exception in checkout stage \r\n"+e
        currentBuild.result = 'FAILURE'
    }

	if(currentBuild.result != 'FAILURE')
	{
		stage 'Install Software'
		try
		{
            //Install NodeJS
            try
            {
                env.PATH = "C:\\tools\\jenkins.plugins.nodejs.tools.NodeJSInstallation\\nodejs-16.17.1;${env.PATH}"
                def globalNodeJSVersion="v16.17.1"
                def currentNodeJSVersion = bat returnStdout: true, script: 'node --version'
                
                echo "Current NodeJS Version"
                echo currentNodeJSVersion;
                
                if(currentNodeJSVersion.indexOf(globalNodeJSVersion) > -1)
                {
                    echo "NodeJS exist with version"
                    echo currentNodeJSVersion;
                }
                else
                {
                    echo "Installation NodeJS started..."
                    nodejs(nodeJSInstallationName: 'nodejs-16.17.1') {
                        bat 'npm config ls'
                    }
                    
                    env.PATH = "C:\\tools\\jenkins.plugins.nodejs.tools.NodeJSInstallation\\nodejs-16.17.1;${env.PATH}"
                    bat 'npm cache clean --force'
                    bat 'npm install -g gulp'
                    bat 'npm install -g sass-lint'
                    bat 'npm install -g eslint'
                    bat 'npm install -g webpack-cli'
                    bat 'npm install -g webpack'
                    echo "Installation NodeJS completed successfully..."
                }
            }
            catch(Exception e)
            {
                echo "Installation NodeJS started..."
                nodejs(nodeJSInstallationName: 'nodejs-16.17.1') {
                    bat 'npm config ls'
                }
            
                env.PATH = "C:\\tools\\jenkins.plugins.nodejs.tools.NodeJSInstallation\\nodejs-16.17.1;${env.PATH}"
                bat 'npm cache clean --force'
                bat 'npm install -g gulp'
                bat 'npm install -g sass-lint'
                bat 'npm install -g eslint'
                bat 'npm install -g webpack-cli'
                bat 'npm install -g webpack'
                echo "Installation NodeJS completed successfully..."
            }

			//Set Time Zone
			echo "Setting Time Zone as India Standard Time..."
			bat 'tzutil /s "India Standard Time"'
			echo "Time Zone changed successfully..."
		}
		catch(Exception e)
		{
			echo "Exception in Install Software stage \r\n"+e
			currentBuild.result = 'FAILURE'
		}
	}
	
	if(currentBuild.result != 'FAILURE')
	{
		stage 'Build Source'
		try
		{
	    	gitlabCommitStatus("Build")
			{
                bat 'powershell.exe -ExecutionPolicy ByPass -File build/build.ps1 -Script '+env.WORKSPACE+"/build/build.cake -Target build -NugetServerUrl "+env.nugetserverurl + " --settings_skipverification=true"
            }
            def files = findFiles(glob: '**/cireports/errorlogs/*.txt')

            if(files.size() > 0)
            {
                currentBuild.result = 'FAILURE'
            }
    	}
		catch(Exception e)
    	{
        	echo "Exception in build source stage \r\n"+e
			currentBuild.result = 'FAILURE'
    	}
	}

	if(currentBuild.result != 'FAILURE')
	{
		stage 'Code violation'
		try
		{
			gitlabCommitStatus("Code violation")
			{
				bat 'powershell.exe -ExecutionPolicy ByPass -File build/build.ps1 -Script '+env.WORKSPACE+"/build/build.cake -Target codeviolation"+ " -settings_skipverification=true"
			}
		}
		catch(Exception e)
		{
			echo "Exception in code violation stage \r\n"+e
			currentBuild.result = 'FAILURE'
		}
	}
	
	if(currentBuild.result != 'FAILURE' && env.publishBranch.contains(githubSourceBranch))
 	{
	 	stage 'Publish'
	 	try
	 	{
	     	gitlabCommitStatus("Publish")
		 	{
				bat 'powershell.exe -ExecutionPolicy ByPass -File build/build.ps1 -Script '+env.WORKSPACE+"/build/build.cake -Target publish -buildNumber "+env.BUILD_NUMBER+" -sourceBranch "+githubSourceBranch+" -nugetapikey "+env.nugetapikey+' -revisionNumber '+env.revisionNumber+' -nugetserverurl '+env.nexusnugetserverurl+" -StudioVersion "+env.studio_version
	 	 	}
     	}
	  	catch(Exception e) 
     	{
			currentBuild.result = 'FAILURE'
     	}
	}

	stage 'Delete Workspace'
    
    // Archiving artifacts when the folder was not empty
    if(fileExists('cireports'))
    {
        archiveArtifacts artifacts: 'cireports/', excludes: null
    }
    
    step([$class: 'WsCleanup'])
}
}
}
