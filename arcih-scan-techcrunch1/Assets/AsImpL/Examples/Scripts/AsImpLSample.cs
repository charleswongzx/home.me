using UnityEngine;

namespace AsImpL
{
    namespace Examples
    {
        /// <summary>
        /// Demonstrate how to load a model with ObjectImporter.
        /// </summary>
        public class AsImpLSample : MonoBehaviour
        {
			public string filePath = "models/OBJ_test/eb_nightstand_01.obj";
			public string filePath2 = "models/OBJ_test/eb_nightstand_01.obj";
            public ImportOptions importOptions = new ImportOptions();
            private ObjectImporter objImporter;

            private void Awake()
            {
#if (UNITY_ANDROID || UNITY_IPHONE)
                filePath = Application.persistentDataPath + "/" + filePath;
#endif
                objImporter = gameObject.AddComponent<ObjectImporter>();

		

            }

			public GameObject walls;
            private void Start()
            {
                objImporter.ImportModelAsync("Walls", filePath, null, importOptions);
				objImporter.ImportModelAsync("Floor", filePath2, null, importOptions);



            }
				
        }

    }
}
