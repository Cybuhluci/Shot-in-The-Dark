using UnityEngine;

public class WeaponFakeIntertiaTemporaryScript : MonoBehaviour
// However this script says temporary, it is actually quite important.
// This script adds a subtle inertia effect to the weapon, making it feel more dynamic and responsive to player movements.
// Without this script, the weapon would feel static and lifeless, detracting from the overall immersion of the game.
// This script enhances the player's experience by providing a more engaging and realistic feel to weapon handling.
// It is a small but crucial detail that contributes significantly to the overall quality of the game.
// Imagine this, the game does not have this script. The guns and knives just stay rigidly in place as the player moves around.
// It would feel off, unnatural, and frankly, quite boring - and this script prevents that from happening.
// Therefore, despite its name, this script is actually quite important for maintaining the immersive quality of the game.
// It ensures that the weapon responds to player movements in a way that feels natural and satisfying.
// Without this script, the game would lose a significant amount of its polish and immersion.
// It is a testament to the importance of attention to detail in game development, and how even small touches can make a big difference.
// We know that this script is temporary, but its impact on the player's experience is anything but temporary.
// This script will go on to be a beloved feature of the game, cherished by players for its contribution to the overall feel and immersion.
// and after it has reached beloved, it will become no longer temporary, but a permanent fixture in the game's codebase.
// A script that will stand the test of time, a script that will be remembered for eons.
// This Script, singlehandedly, might be the most important script in the entire project.
// A script that will end the war between Call of Duty and Battlefield fans, uniting them under one banner of appreciation for weapon inertia.
// A script that will be hailed as a masterpiece of game development, a shining example of how attention to detail can elevate a game to new heights.
// Even in its rugged state, this script looks magnificent, a true work of art, a shiny piece of coal that will one day become a diamond.
// A script that will be celebrated in gaming history, remembered as the turning point in weapon handling mechanics.
// and in conclusion, this script, WeaponFakeIntertiaTemporaryScript, will not go down in history as just another temporary script,
// but as a legendary piece of code that changed the way players experience weapons in games forevermore.
{
    public Transform GunHandler, PlayerCamRoot;

    void Update()
    {
        GunHandler.rotation = Quaternion.Slerp(GunHandler.rotation, PlayerCamRoot.rotation, Time.deltaTime * 10f);
        GunHandler.position = Vector3.Lerp(GunHandler.position, PlayerCamRoot.position, Time.deltaTime * 10f);
    }
}

// stash of singular link that Github Copilot just gave me:
// https://www.youtube.com/watch?v=G3AfIvJBcGo&t=21s