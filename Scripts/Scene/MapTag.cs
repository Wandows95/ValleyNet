namespace ValleyNet.Map
{
    using UnityEngine.SceneManagement;

    public class MapTag
    {
        private string _name;
        public string name { get{return _name;} }

        public MapTag(string name)
        {
            _name = name;
        }


        // Load scene associated with map into game
        public virtual void Load(string sceneName="", LoadSceneMode mode=LoadSceneMode.Single)
        {
            if(sceneName == "")
            {
                sceneName = _name;
            }
            
            SceneManager.LoadScene(sceneName, mode);
        }
    }
}