﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BH.Engine.Adapter.Karamba3D
{
    public static class K3dLogger
    {
        private static List<string> m_allMessages = new List<string>();
        private static List<string> m_reportedErrors = new List<string>();
        private static List<string> m_reportedWarnings = new List<string>();
        private static List<string> m_reportedNotes = new List<string>();

        private enum MessageLevel
        {
            ERROR,
            Warning,
            Note
        }

        public static void RecordError(string error, bool doNotRepeat = false, bool appendToPrevious = false)
        {
            RecordMessage(MessageLevel.ERROR, error, doNotRepeat, appendToPrevious);
        }

        public static void RecordWarning(string warning, bool doNotRepeat = false, bool appendToPrevious = false)
        {
            RecordMessage(MessageLevel.Warning, warning, doNotRepeat, appendToPrevious);
        }

        public static void RecordNote(string note, bool doNotRepeat = false, bool appendToPrevious = false)
        {
            RecordMessage(MessageLevel.Note, note, doNotRepeat, appendToPrevious);
        }

        private static void RecordMessage(MessageLevel messageLevel, string message, bool doNotRepeat = false, bool appendToPrevious = false)
        {
            List<string> m_relevantMessages = GetMessages(messageLevel);

            if (doNotRepeat && m_relevantMessages.Contains(message))
                return;

            Console.WriteLine(message);
            ReportToUI(messageLevel, message);

            if (appendToPrevious && m_relevantMessages.Any())
                m_relevantMessages.AppendToLast(message);
            else
                m_relevantMessages.Add(message);

            m_allMessages.Add($"{DateTime.Now}\t{messageLevel}: {message}");
        }

        private static List<string> GetMessages(MessageLevel messageLevel)
        {
            if (messageLevel == MessageLevel.ERROR)
                return m_reportedErrors;

            if (messageLevel == MessageLevel.Warning)
                return m_reportedWarnings;

            if (messageLevel == MessageLevel.Note)
                return m_reportedNotes;

            return new List<string>();
        }

        private static void ReportToUI(MessageLevel messageLevel, string message)
        {
            if (messageLevel == MessageLevel.ERROR)
                BH.Engine.Base.Compute.RecordError(message);

            if (messageLevel == MessageLevel.Warning)
                BH.Engine.Base.Compute.RecordWarning(message);

            if (messageLevel == MessageLevel.Note)
                BH.Engine.Base.Compute.RecordNote(message);
        }

        public static List<string> GetErrors()
        {
            return new List<string>(m_reportedErrors);
        }

        public static List<string> GetWarnings()
        {
            return new List<string>(m_reportedWarnings);
        }

        public static List<string> GetNotes()
        {
            return new List<string>(m_reportedNotes);
        }

        public static List<string> GetAllMessages()
        {
            return new List<string>(m_allMessages);
        }

        public static void SaveLogToDisk(string filePath)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            File.WriteAllLines(filePath, m_allMessages);
        }

        public static void Clean()
        {
            m_allMessages = new List<string>();
            m_reportedErrors = new List<string>();
            m_reportedWarnings = new List<string>();
            m_reportedNotes = new List<string>();
        }

        private static void AppendToLast(this IList<string> set, string toAppend)
        {
            string lastMessage = set.Last();
            set.Remove(lastMessage);
            lastMessage += Environment.NewLine + toAppend;
            set.Add(lastMessage);
        }
    }
}