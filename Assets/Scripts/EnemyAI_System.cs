using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public static class EnemyAI_System
{
    public static void Update(ref EnemyCharacter enemy)
    {
        Main main = Main.Singleton;
        ref Character character = ref enemy.character;

        if (main.isPlayerAlive)
        {
            character.isUsingLookPositionInput = true;
            character.lookPositionInput = main.playerCharacter.character.transform.position;
            
            RaycastHit hit;
            if (Physics.SphereCast(character.transform.position, 0.5f, character.transform.forward, out hit, 1.0f))
            {
                Vector3 projection = Vector3.ProjectOnPlane(character.transform.forward, hit.normal).normalized;
                character.lookPositionInput = character.transform.position + projection;
            }
            
            character.moveInput = new Vector2(0, 1);
            character.shootInput =
                Vector3.Distance(character.transform.position, character.lookPositionInput) <=
                (character.ActiveGun.isMelee ? ShootSystem.MeleeRange : ShootSystem.ShootRange);
            character.jumpInput = false;
            character.reloadInput = character.ActiveGun.currentAmmo == 0;
        }
        else
        {
            character.isUsingLookPositionInput = false;
            character.lookInput = new Vector2();
            character.moveInput = new Vector2();
            character.shootInput = false;
            character.jumpInput = false;
            character.reloadInput = false;
        }
    }

    // For some reason batching this shit doesn't work. :(
    public static void UpdateAvoidance(ref VersionedList<EnemyCharacter> enemyCharacters)
    {
        int enemyCount = enemyCharacters.list.Count;
        
        NativeArray<RaycastHit> results =  new NativeArray<RaycastHit>(enemyCount, Allocator.TempJob);
        NativeArray<SpherecastCommand> commands = new NativeArray<SpherecastCommand>(enemyCount, Allocator.TempJob);

        int i = 0;
        for (i = 0; i < enemyCount; i += 1)
        {
            Vector3 origin = enemyCharacters[i].character.transform.position;
            Vector3 direction = enemyCharacters[i].character.transform.forward;
            // Assumes capsule radius is 0.5.
            commands[i] = new SpherecastCommand(origin, 0.5f, direction, QueryParameters.Default, 1.5f);
        }
        
        JobHandle handle = SpherecastCommand.ScheduleBatch(commands, results, 1, 1);
        handle.Complete();

        i = 0;
        foreach (ref EnemyCharacter enemy in enemyCharacters)
        {
            RaycastHit hit = results[i];
            if (hit.collider != null)
            {
                ref Character character = ref enemy.character; 
                Vector3 projection = Vector3.ProjectOnPlane(character.transform.forward, hit.normal);
                character.lookPositionInput = character.transform.position + projection;
            }

            i += 1;
        }
        

        results.Dispose();
        commands.Dispose();

    }
}
