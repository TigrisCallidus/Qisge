using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstrumentObject : MonoBehaviour
{
    public AudioSource Source;
    public bool IsRepeating = true;

    int currentBar = -1;
    int currentTick = 0;
    int ticksPerBar = 0;
    int songLength = 0;
    int trackLength = 0;

    public void Synchronize(int startBar, int ticksPerBar, int songLength, bool repeating=true) {

        if (currentBar!=startBar) {
            currentBar = startBar;
        }

        currentTick = 0;

        this.ticksPerBar = ticksPerBar;
        this.songLength = songLength;

        IsRepeating = repeating;
    }

    public void Tick() {
        currentTick++;
        if (currentTick>= ticksPerBar) {
            currentTick = 0;
            currentBar++;

            if (IsRepeating && currentBar>= trackLength) {
                currentBar = 0;
            } else if (currentBar>=songLength) {
                currentBar = 0;
            }
        }
    }

    public void CheckSound() {

    }

}
