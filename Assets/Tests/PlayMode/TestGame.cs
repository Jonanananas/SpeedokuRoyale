using System.Collections;

using NUnit.Framework;

using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

using TMPro;

namespace TestGame {
    public class TestSinglePlayerPlayButtons {
        TestUtility util = new TestUtility();
        [SetUp]
        public void SetUp() {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
        }
        [UnityTest]
        public IEnumerator TestMainMenuPlayButtons() {
            util.FindAndClickButton("Play Button", false);

            util.FindAndClickButton("Solo Button", false);
            yield return new WaitForSeconds(0.1f);
        }
    }
    public class TestUserProfileFunctions {
        TestUtility util = new TestUtility();
        string username, password, newPassword;
        [SetUp]
        public void SetUp() {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
        }
        [UnityTest, Order(1)]
        public IEnumerator TestProfileCreation() {
            // Get and press login button
            util.FindAndClickButton("Login Button", true);

            // Get and press register button
            util.FindAndClickButton("Register Button", false);

            username = System.Guid.NewGuid().ToString();
            util.FindTMP_InputFieldAndInsertText("Username", username);

            password = System.Guid.NewGuid().ToString();
            util.FindTMP_InputFieldAndInsertText("Password", password);

            util.FindTMP_InputFieldAndInsertText("Password Repeat", password);

            // Get and press register button
            util.FindAndClickButton("Register Button", true);

            yield return new WaitForSeconds(0.5f);

            util.FindAndClickButton("MainMenuButton", false);
            // yield return new WaitForSeconds(0.1f);

            // Logout
            util.FindAndClickButton("Login Button", true);
            yield return new WaitForSeconds(0.5f);
        }
        [UnityTest, Order(2)]
        public IEnumerator TestLogIn() {
            // Login
            util.FindAndClickButton("Login Button", true);
            util.FindTMP_InputFieldAndInsertText("Username", username);
            util.FindTMP_InputFieldAndInsertText("Password", password);
            util.FindAndClickButton("Login Button", true);
            yield return new WaitForSeconds(0.5f);
            util.FindAndClickButton("MainMenuButton", false);
        }
        [UnityTest, Order(3)]
        public IEnumerator TestChangingPassword() {
            // Go to user settings
            util.FindAndClickButton("Statistics Button", false);
            util.FindAndClickButton("Profile Button", false);
            util.FindAndClickButton("UserSettings Button", false);

            // Change password
            util.FindAndClickButton("Change Password", false);
            util.FindTMP_InputFieldAndInsertText("Password", password);
            password = System.Guid.NewGuid().ToString();
            util.FindTMP_InputFieldAndInsertText("NewPassword", password);
            util.FindTMP_InputFieldAndInsertText("NewPasswordRepeat", password);

            util.FindAndClickButtonWithTwoFunctions("Confirm Button");
            yield return new WaitForSeconds(0.5f);

            // Go back to main menu
            util.FindAndClickButton("Back Button", false);
            util.FindAndClickButton("Back Button", false);
            util.FindAndClickButton("Back Button", false);

            // Logout
            util.FindAndClickButton("Login Button", true);
            yield return new WaitForSeconds(0.5f);

            TestLogIn();
        }
        [UnityTest, Order(4)]
        public IEnumerator TestProfileDeletion() {
            // Go to user settings
            util.FindAndClickButton("Statistics Button", false);
            util.FindAndClickButton("Profile Button", false);
            util.FindAndClickButton("UserSettings Button", false);

            // Delete account
            util.FindAndClickButton("Delete Account", false);
            util.FindAndClickButtonWithTwoFunctions("Delete Button");
            yield return new WaitForSeconds(0.5f);

            // Go back to main menu
            util.FindAndClickButton("Back Button", false);
            util.FindAndClickButton("Back Button", false);
            util.FindAndClickButton("Back Button", false);

            util.FindAndClickButton("Login Button", true);
            util.FindAndClickButton("Back Button", false);
        }
    }

    class TestUtility {
        bool clicked = false;
        private void Clicked() {
            clicked = true;
        }
        public void FindAndClickButton(string buttonName, bool useOnPointerUp) {
            GameObject go = GameObject.Find(buttonName);
            Assert.NotNull(go);

            Button button = go.GetComponent<Button>();

            if (useOnPointerUp) {
                ButtonInterface btn = go.GetComponent<ButtonInterface>();
                btn.TryToPress();
                return;
            }

            button.onClick.AddListener(Clicked);
            button.onClick.Invoke();
            Assert.IsTrue(clicked);
            clicked = false;
        }
        public void FindAndClickButtonWithTwoFunctions(string buttonName) {
            GameObject go = GameObject.Find(buttonName);
            Assert.NotNull(go);

            Button button = go.GetComponent<Button>();

            ButtonInterface btn = go.GetComponent<ButtonInterface>();
            btn.TryToPress();

            button.onClick.AddListener(Clicked);
            button.onClick.Invoke();
            Assert.IsTrue(clicked);
            clicked = false;
        }
        public void FindTMP_InputFieldAndInsertText(string inputFieldName, string text) {
            GameObject go = GameObject.Find(inputFieldName);
            Assert.NotNull(go);
            TMP_InputField inputField = go.GetComponent<TMP_InputField>();
            inputField.text = text;
        }
    }
}