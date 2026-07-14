pipeline {
    agent any

    environment {
        DOTNET_CLI_TELEMETRY_OPTOUT = '1'
        DOTNET_NOLOGO = '1'
    }

    stages {
        stage('Checkout') {
            steps {
                checkout scm
            }
        }

        stage('Configure for CI') {
            steps {
                // Self-configuring: this pipeline always runs against the CI
                // config (headless, no Grid), regardless of whatever
                // appsettings.json was last set to locally.
                sh 'cp appsettings.ci.json appsettings.json'
            }
        }

        stage('Restore') {
            steps {
                sh 'dotnet restore'
            }
        }

        stage('Build') {
            steps {
                sh 'dotnet build --no-restore'
            }
        }

        stage('Test') {
            steps {
                sh 'dotnet test --no-build --logger "trx;LogFileName=test-results.trx"'
            }
        }
    }

    post {
        always {
            junit testResults: '**/test-results.trx', allowEmptyResults: true

            publishHTML(target: [
                allowMissing: true,
                alwaysLinkToLastBuild: true,
                keepAll: true,
                reportDir: 'artifacts/extent',
                reportFiles: 'extent-report.html',
                reportName: 'Extent Report'
            ])

            archiveArtifacts artifacts: 'artifacts/screenshots/**', allowEmptyArchive: true
            archiveArtifacts artifacts: 'artifacts/logs/**', allowEmptyArchive: true

            script {
                if (fileExists('bin/Debug/net10.0/allure-results')) {
                    allure includeProperties: false,
                        jdk: '',
                        results: [[path: 'bin/Debug/net10.0/allure-results']]
                }
            }
        }
    }
}