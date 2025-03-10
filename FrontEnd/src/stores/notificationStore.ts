import { defineStore } from "pinia";
import { ref, watch } from "vue";

export const useNotificationStore = defineStore("notification", () => {
  const notifications = ref<string[]>(
    JSON.parse(localStorage.getItem("notifications") || "[]")
  );

  const addNotification = (message: string) => {
    notifications.value.unshift(message);
    localStorage.setItem("notifications", JSON.stringify(notifications.value));
  };

  const clearNotifications = () => {
    notifications.value = [];
    localStorage.removeItem("notifications");
  };

  watch(notifications, (newVal) => {
    localStorage.setItem("notifications", JSON.stringify(newVal));
  });

  return { notifications, addNotification, clearNotifications };
});
