@NonCPS

// Get localtime
def getLocalTime() {
  Date date = new Date()
  String datePart = date.format('yyyyMMdd')
  String timePart = date.format('HHmmss')
  return datePart + timePart
}

def timeStamp = getLocalTime()
println('timeStamp: ' + timeStamp)

def branch_name = 'jonathan-dev'
println('branch_name: ' + timeStamp)

// Your Github URL
def build_repo = 'https://github.com/Jonanananas/SpeedokuRoyale.git'
def result = currentBuild.currentResult.toLowerCase()

pipeline {
  // Master Jenkins
  agent any

  // Initialize environment params
  environment {
    UNITY_PATH = '/InstallFolder/Editor/Unity'
    app_mode = 'mono'  // mono or IL2CPP
    repo = "${build_repo}"
    branch = "${branch_name}"
    workingDir = '/var/lib/jenkins/workspace/UnityProject'
  }
  stages {
    // stage('PlayMode Test') {
    //   steps {
    //       catchError(buildResult: 'FAILURE', stageResult: 'FAILURE') {
    //         sh """
    //           sudo ${UNITY_PATH} -batchmode -projectPath ${workingDir} -runTests -testResults ${workingDir}/CI/results.xml -testPlatform PlayMode -nographics;\
    //         """
    //       }
    //   }
    // }
    stage('Publish NUnit Test Report') {
      steps {
        nunit testResultsPattern: "${workingDir}/CI/results.xml",
        failIfNoResults : true
      }
    }
    // stage('Build') {
    //   steps {
    //     script {
    //       if (currentBuild.currentResult != 'FAILURE') {
    //         catchError(buildResult: 'FAILURE', stageResult: 'FAILURE') {
    //           sh """
    //             sudo ${UNITY_PATH} -batchmode -projectPath ${workingDir} -buildTarget WebGL -executeMethod BuilderUtility.BuildWebGL -nographics -quit;\
    //         """
    //         }
    //       } else {
    //         catchError(buildResult: 'FAILURE', stageResult: 'FAILURE') {
    //           sh 'exit 2'
    //         }
    //       }
    //     }
    //   }
    // }

    // stage('Send A Discord message') {
    //   steps {
    //     discordSend webhookURL: 'https://discord.com/api/webhooks/1042527642915713094/Vm4aIEwDrTnH2j0fDozOdNdlrKaMXwjQMlNCBGrjf6gml01_2UsIaSr_iUNX-iYbUHZI',
    //         title: "${env.JOB_BASE_NAME} #${env.BUILD_NUMBER}",
    //         result: currentBuild.currentResult,
    //         description:  """**Build:** ${env.BUILD_NUMBER}
    //                       **Branch:** ${branch_name}
    //                       **Status:** ${currentBuild.currentResult}\n\u2060""", /* word joiner character forces a blank line */
    //         // enableArtifactsList: true,
    //         showChangeset: true
    //   }
    // }
  }
}
