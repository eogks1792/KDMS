﻿using KdnTcpClient.Shared.Interface;
using System.Runtime.CompilerServices;

namespace KdnTcpClient.Util
{
    public class ApiLogger : IApiLogger
    {
        private readonly ILogger<ApiLogger> _logger;

        public ApiLogger(ILogger<ApiLogger> logger) => this._logger = logger;
        public void Log(LogLevel logLevel, string message, [CallerMemberName] string membername = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, params object[] args)
        {
            _logger.Log(logLevel, message, membername, filePath, lineNumber, args);
        }

        public void LogDebug(string message, [CallerMemberName] string membername = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, params object[] args)
        {
            _logger.LogDebug(
                    CreateLogMessage(message, membername, filePath, lineNumber), args);
        }

        public void LogError(string message, [CallerMemberName] string membername = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, params object[] args)
        {
            _logger.LogError(
                    CreateLogMessage(message, membername, filePath, lineNumber), args);
        }

        public void LogError(Exception exception, string message, [CallerMemberName] string membername = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, params object[] args)
        {
            _logger.LogError(exception,
                    CreateLogMessage(message, membername, filePath, lineNumber), args);
        }

        public void LogInformation(string message, [CallerMemberName] string membername = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, params object[] args)
        {
            _logger.LogInformation(
                    CreateLogMessage(message, membername, filePath, lineNumber), args);
        }

        public void LogTrace(string message, [CallerMemberName] string membername = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, params object[] args)
        {
            _logger.LogTrace(
                    CreateLogMessage(message, membername, filePath, lineNumber), args);
        }

        public void LogWarning(string message, [CallerMemberName] string membername = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, params object[] args)
        {
            _logger.LogWarning(CreateLogMessage(message, membername, filePath, lineNumber), args);
        }

        private string CreateLogMessage(string message,
            string memberName, string filePath, int lineNumber) =>
            $"[{memberName}] {message}";
        //$"[{memberName}] {message}. \nfilePath = {filePath} : line = {lineNumber}";
    }
}
