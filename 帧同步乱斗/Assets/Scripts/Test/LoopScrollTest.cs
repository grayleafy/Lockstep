using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopScrollTest : BasePanel
{
    public LoopScrollList loopScrollList;

    private void Start()
    {
        ModelList<Room> roomList = new ModelList<Room>();
        roomList.Add(new Room() { Name = "leaf" });
        roomList.Add(new Room() { Name = "gray" });
        roomList.Add(new Room() { Name = "465" });

        loopScrollList.BindData(roomList);
    }
}
