// Import Firebase SDKs (nếu bạn nhúng file này từ <script>, phần import sẽ bỏ qua)
import { initializeApp } from "https://www.gstatic.com/firebasejs/10.12.0/firebase-app.js";
import { getAnalytics } from "https://www.gstatic.com/firebasejs/10.12.0/firebase-analytics.js";

// Firebase config của bạn
const firebaseConfig = {
  apiKey: "AIzaSyC5ISjq9Cr7re6m8IG9UANw7WDjP7UCjKw",
  authDomain: "dalgona-3d7a5.firebaseapp.com",
  projectId: "dalgona-3d7a5",
  storageBucket: "dalgona-3d7a5.appspot.com",
  messagingSenderId: "498024780224",
  appId: "1:498024780224:web:8de117bf8f6cf110a67825",
  measurementId: "G-8TYG9LEC6B"
};

// Khởi tạo Firebase
const app = initializeApp(firebaseConfig);
const analytics = getAnalytics(app);

// Export app nếu cần dùng ở file khác
export { app };
