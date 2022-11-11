using System.Collections;

using NUnit.Framework;

using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
namespace TestGame {
    public class PlayModeTestGame {
        bool clicked = false;
        [SetUp]
        public void SetUp() {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
        }
        //test2
        [UnityTest]
        public IEnumerator TestMenu() {
            var gameObject = new GameObject();

            // Check if a play button exists
            string name = "Play Buttonadw"; // Button Name
            GameObject startButton = GameObject.Find(name);
            Assert.NotNull(startButton);

            // Check if the play button is clickable
            var setupButton = startButton.GetComponent<Button>();
            setupButton.onClick.AddListener(Clicked);
            setupButton.onClick.Invoke();
            Assert.IsTrue(clicked);
            yield return new WaitForSeconds(0.1f);
        }
        private void Clicked() {
            clicked = true;
        }
    }
}