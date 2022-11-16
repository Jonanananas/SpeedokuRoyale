@NonCPS

// Get localtime
def getLocalTime() {
  Date date = new Date()
  String datePart = date.format("yyyyMMdd")
  String timePart = date.format("HHmmss")
  return datePart + timePart
}

def timeStamp=getLocalTime()
println("timeStamp: "+ timeStamp)

def branch_name = "dev"
println("branch_name: "+ timeStamp)

// Pipeline name
def jobName="UnityProject"
// Your Github URL
def build_repo="https://github.com/Jonanananas/SpeedokuRoyale.git"
// Your Unity Version
// Ex: "2020.1.15f2"
def unity_version="2021.3.8f1"

pipeline {
   // Master Jenkins
   agent any

   // Initialize environment params
   environment{
       UNITY_PATH="/InstallFolder/Editor/Unity"
       app_mode="mono"  // mono or IL2CPP
       repo="${build_repo}"
       branch="${branch_name}"
       workingDir="/var/lib/jenkins/workspace/UnityProject"
   }
    stages {
        stage('Pull Branch') {
            steps {
              sh """sudo git pull ${repo} ${branch};\
                """
            }
        }

        stage('PlayMode Test') {
            steps {
              sh """sudo ${UNITY_PATH} -batchmode -projectPath ${workingDir} -runTests -testResults ${workingDir}/CI/results.xml -testPlatform PlayMode -nographics;\
                """
            }
        }
    }
}