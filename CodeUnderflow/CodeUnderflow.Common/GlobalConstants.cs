﻿namespace CodeUnderflow.Common
{
    public static class GlobalConstants
    {
        public const string AdminRoleName = "Administrator";
        public const string ModeratorRoleName = "Moderator";
        public const string QuestionVoteCookiesName = "QuestionsVoted";
        public const int MaxItemsPerPage = 4;
        public static readonly string[] AvailableRoles = { AdminRoleName, ModeratorRoleName };
    }
}