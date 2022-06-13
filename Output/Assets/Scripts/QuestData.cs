using System;
using RagnarEngine;

[System.Serializable]
public class QuestData
{
    uint id;
    string name;
    string description;
    public QuestState state;
    QuestType type;

    public QuestData(Quest quest)
    {
        id = quest.GetQuestId();
        name = quest.GetQuestName();
        description = quest.GetQuestDescription();
        state = quest.GetQuestState();
        type = quest.GetQuestType();
    }
}
