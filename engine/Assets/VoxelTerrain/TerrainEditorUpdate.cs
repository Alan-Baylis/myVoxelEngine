using UnityEngine;
using System.Collections;
using VoxelEngine;
[ExecuteInEditMode]
public class TerrainEditorUpdate : MonoBehaviour {
	public VoxelTerrainEngine Engine;
	public void OnDrawGizmos(){
		if(UnityEditor.EditorApplication.isPlaying==false)
			Engine.playMode= EditorPlay.isEditor;
		if(Engine!=null && Engine.playMode== EditorPlay.isEditor){
			Engine.UpdateTerrain();
		}
		if(Engine==null)
			Engine = GetComponent<VoxelTerrainEngine>();
	}
}
