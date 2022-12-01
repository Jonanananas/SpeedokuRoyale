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

def branch_name = "jonathan-dev"
println("branch_name: "+ timeStamp)

// Pipeline name
def jobName="UnityProject"
// Your Github URL
def build_repo="https://github.com/Jonanananas/SpeedokuRoyale.git"
// Your Unity Version
// Ex: "2020.1.15f2"
def unity_version="2021.3.8f1"

def result = currentBuild.currentResult.toLowerCase()

pipeline {
   // Master Jenkins
   agent any

   // Initialize environment params
   environment{
       UNITY_PATH="/home/InstallFolder/Editor/Unity"
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

        // stage('PlayMode Test') {
        //     steps {
        //       sh """git checkout ${branch};\
        //             sudo ${UNITY_PATH} -batchmode -projectPath ${workingDir} -runTests -testResults ${workingDir}/CI/results.xml -testPlatform PlayMode -nographics;\
        //         """
        //     }
        // }
        stage('Build') {
          steps {
            script {
              sh """cd ${workingDir}/builds;\
                  sudo ${UNITY_PATH} -batchmode -projectPath ${workingDir} -buildTarget WebGL -executeMethod BuilderUtility.BuildWebGL -nographics -quit;\
                """
            }
          }
        }

        stage('Send A Discord message') {
          steps {
            discordSend webhookURL: "https://discord.com/api/webhooks/1042527642915713094/Vm4aIEwDrTnH2j0fDozOdNdlrKaMXwjQMlNCBGrjf6gml01_2UsIaSr_iUNX-iYbUHZI",
            title: "${env.JOB_BASE_NAME} #${env.BUILD_NUMBER}",
            result: currentBuild.currentResult,
            description:  """**Build:** ${env.BUILD_NUMBER}
                          **Branch:** ${branch_name}
                          **Status:** ${result}\n\u2060""", /* word joiner character forces a blank line */
            // enableArtifactsList: true,
            showChangeset: true
          }
        }
    }
}