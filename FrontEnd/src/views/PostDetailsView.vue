<script setup lang="ts">
import { ref, onMounted } from "vue";
import axios from "axios";
import { useRoute } from "vue-router";

// 🌐 API URL
const API_BASE_URL = "https://localhost:7232/api";

// 📌 Get postId from route
const route = useRoute();
const postId = ref<number>(Number(route.params.id));

// 📝 Types for Post and Comment
interface User {
  id: number;
  userName?: string;
  fullName?: string;
  avatar?: string;
}

interface Post {
  id: number;
  caption: string;
  content: string;
  createdDate: string;
  likesCount: number;
  imageUrl?: string;
  user?: User;
}

interface Comment {
  id: number;
  postId: number;
  user?: User;
  content: string;
  createdDate: string;
}

// 📌 State for post & comments
const post = ref<Post | null>(null);
const comments = ref<Comment[]>([]);
const newComment = ref<string>("");
const isLoading = ref<boolean>(true);
const isSubmitting = ref<boolean>(false);
const isLiked = ref<boolean>(false);

// 🔄 Format date to a more readable format
const formatDate = (dateString: string) => {
  const date = new Date(dateString);
  return new Intl.DateTimeFormat("default", {
    year: "numeric",
    month: "short",
    day: "numeric",
    hour: "2-digit",
    minute: "2-digit",
  }).format(date);
};

// 🟢 Fetch post details
const fetchPost = async () => {
  try {
    isLoading.value = true;
    console.log(
      "🔍 Fetching API:",
      `${API_BASE_URL}/posts/get/${postId.value}`
    );
    const response = await axios.get<{ result: Post }>(
      `${API_BASE_URL}/posts/get/${postId.value}`
    );
    post.value = response.data.result;
  } catch (error) {
    console.error("❌ Error fetching post:", error);
  } finally {
    isLoading.value = false;
  }
};

// 🟢 Fetch comments list
const fetchComments = async () => {
  try {
    console.log(
      "🔍 Fetching API:",
      `${API_BASE_URL}/PostInfo/comments/${postId.value}`
    );
    const response = await axios.get<{ result: Comment[] }>(
      `${API_BASE_URL}/PostInfo/comments/${postId.value}`
    );
    comments.value = response.data.result;
  } catch (error) {
    console.error("❌ Error fetching comments:", error);
  }
};

// ❤️ Like the post
const likePost = async () => {
  if (isLiked.value) return;

  try {
    await axios.post(`${API_BASE_URL}/PostInfo/like/${postId.value}`);
    post.value!.likesCount += 1;
    isLiked.value = true;
  } catch (error) {
    console.error("❌ Error liking post:", error);
  }
};

// Report post function
const reportPost = () => {
  alert("Report functionality will be implemented in a future update.");
};

// Share post function
const sharePost = () => {
  navigator.clipboard.writeText(window.location.href);
  alert("Link copied to clipboard!");
};

// ✍️ Submit new comment
const submitComment = async () => {
  if (!newComment.value.trim()) return;

  try {
    isSubmitting.value = true;
    await axios.post(`${API_BASE_URL}/PostInfo/comment`, {
      postId: postId.value,
      content: newComment.value,
    });
    newComment.value = "";
    fetchComments();
  } catch (error) {
    console.error("❌ Error submitting comment:", error);
  } finally {
    isSubmitting.value = false;
  }
};

// 🔄 Call API when component mounts
onMounted(() => {
  fetchPost();
  fetchComments();
});
</script>

<template>
  <div class="neon-theme">
    <!-- Header -->
    <header class="site-header">
      <div class="logo">
        <router-link to="/" class="logo-link">
          <span class="logo-text"
            >GTZ<span class="logo-separator">·</span>G</span
          >
        </router-link>
      </div>
      <div class="header-actions">
        <button class="neon-btn blue">
          <span class="btn-icon">🔄</span>
          <span class="btn-text">Nạp Tiền</span>
        </button>
        <button class="neon-btn green">
          <span class="btn-icon">+</span>
          <span class="btn-text">Thêm</span>
        </button>
        <button class="neon-btn icon-only">
          <span class="btn-icon">📅</span>
        </button>
        <button class="neon-btn icon-only">
          <span class="btn-icon">🔔</span>
        </button>
        <div class="user-menu">
          <span class="user-avatar">N</span>
          <span class="user-name">Nguyễn Văn A</span>
          <span class="dropdown-icon">▼</span>
        </div>
      </div>
    </header>

    <div class="layout">
      <!-- Sidebar -->
      <div class="sidebar">
        <nav class="nav-menu">
          <router-link to="/" class="nav-item">
            <span class="nav-icon">🏠</span>
            <span class="nav-text">Home</span>
          </router-link>
          <router-link to="/forum" class="nav-item active">
            <span class="nav-icon">💬</span>
            <span class="nav-text">Forum</span>
          </router-link>
          <router-link to="/marketplace" class="nav-item">
            <span class="nav-icon">🛒</span>
            <span class="nav-text">Marketplace</span>
          </router-link>
          <router-link to="/support" class="nav-item">
            <span class="nav-icon">📞</span>
            <span class="nav-text">Support</span>
          </router-link>
        </nav>
      </div>

      <!-- Main Content -->
      <main class="main-content">
        <!-- Breadcrumb -->
        <div class="breadcrumb">
          <router-link to="/forums">Forums</router-link>
          <span class="breadcrumb-separator">/</span>
          <router-link to="/forums/category">Category</router-link>
          <span class="breadcrumb-separator">/</span>
          <span class="breadcrumb-current">aaaa</span>
        </div>

        <!-- Post Header -->
        <div class="post-header">
          <h1 class="post-title">{{ post?.caption || "aaaa" }}</h1>
          <div class="post-meta">
            Posted
            {{
              post?.createdDate
                ? formatDate(post.createdDate)
                : "Feb 16, 2025, 06:54 AM"
            }}
          </div>
        </div>

        <!-- Loading State -->
        <div v-if="isLoading" class="loading">
          <div class="loading-spinner"></div>
          <span>Loading...</span>
        </div>

        <!-- Post Content -->
        <div v-else class="post-content">
          <!-- Author Info -->
          <div class="author-info">
            <div class="author-avatar">
              <span class="avatar-letter">{{
                post?.user?.fullName?.[0] || "N"
              }}</span>
            </div>
            <div class="author-name">
              {{ post?.user?.fullName || "Nguyễn Văn A" }}
            </div>
            <div class="author-role">Member</div>
          </div>

          <!-- Post Body -->
          <div class="post-body">
            <p>{{ post?.content || "aaaa" }}</p>

            <div v-if="post?.imageUrl" class="post-image">
              <img :src="post.imageUrl" alt="Post Image" />
            </div>

            <!-- Post Actions -->
            <div class="post-actions">
              <button
                class="action-btn like"
                :class="{ active: isLiked }"
                @click="likePost"
              >
                <span class="action-icon">❤️</span>
                <span class="action-count">{{ post?.likesCount || 4 }}</span>
              </button>
              <button class="action-btn share" @click="sharePost">
                <span class="action-text">Share</span>
              </button>
              <button class="action-btn report" @click="reportPost">
                <span class="action-text">Report</span>
              </button>
            </div>
          </div>
        </div>

        <!-- Comments Section -->
        <div class="comments-section">
          <h2 class="section-title">
            Comments<span class="comment-count"
              >({{ comments.length || 3 }})</span
            >
          </h2>

          <!-- Comments List -->
          <div class="comments-list">
            <div
              v-for="(comment, index) in comments.length
                ? comments
                : [
                    {
                      id: 1,
                      content: 'Cũng ok',
                      user: { fullName: 'Ngô Trần Nguyên Quân' },
                      createdDate: 'Mar 9, 2025, 01:37 PM',
                    },
                    {
                      id: 2,
                      content: 'Cũng ok 2',
                      user: { fullName: 'Ngô Trần Nguyên Quân' },
                      createdDate: 'Mar 9, 2025, 01:38 PM',
                    },
                    {
                      id: 3,
                      content: 'Cũng ok 3',
                      user: { fullName: 'Ngô Trần Nguyên Quân' },
                      createdDate: 'Mar 9, 2025, 01:39 PM',
                    },
                  ]"
              :key="comment.id"
              class="comment-item"
            >
              <div class="comment-user">
                <div class="comment-user-avatar">
                  {{ comment.user?.fullName?.[0] || "N" }}
                </div>
              </div>
              <div class="comment-content">
                <div class="comment-header">
                  <span class="comment-author">{{
                    comment.user?.fullName || "Ngô Trần Nguyên Quân"
                  }}</span>
                  <span class="comment-time">{{
                    comment.createdDate
                      ? formatDate(comment.createdDate)
                      : comment.createdDate
                  }}</span>
                </div>
                <div class="comment-text">{{ comment.content }}</div>
              </div>
            </div>
          </div>

          <!-- Comment Form -->
          <div class="comment-form">
            <h3 class="form-title">Leave a comment</h3>
            <div class="form-input">
              <textarea
                v-model="newComment"
                placeholder="Write your comment..."
                class="comment-textarea"
              ></textarea>
            </div>
            <div class="form-actions">
              <button
                class="neon-btn blue post-comment"
                :disabled="!newComment.trim() || isSubmitting"
                @click="submitComment"
              >
                Post Comment
              </button>
            </div>
          </div>
        </div>

        <!-- Related Discussions -->
        <div class="related-discussions">
          <h2 class="section-title">Related Discussions</h2>
          <div class="discussion-list">
            <div class="discussion-item">
              <div class="discussion-info">
                <h3 class="discussion-title">[Planned feature] Soon™</h3>
                <p class="discussion-preview">
                  A brief preview of the discussion content would go here...
                </p>
              </div>
              <div class="discussion-meta">
                <div class="discussion-date">Dec 11, 2024</div>
                <div class="discussion-author">by kouyo</div>
              </div>
            </div>
            <div class="discussion-item">
              <div class="discussion-info">
                <h3 class="discussion-title">
                  Change "Popular New Titles" to "Popular Weekly Titles"
                  [Planned]
                </h3>
                <p class="discussion-preview">
                  A brief preview of the discussion content would go here...
                </p>
              </div>
              <div class="discussion-meta">
                <div class="discussion-date">Feb 19, 2024</div>
                <div class="discussion-author">by kabachi</div>
              </div>
            </div>
          </div>
        </div>
      </main>
    </div>
  </div>
</template>

<style>
/* Base Styles and Variables */
:root {
  --neon-blue: #00c6ff;
  --neon-purple: #7b2cf9;
  --neon-pink: #f43f5e;
  --neon-green: #00ff94;
  --dark-blue: #0f172a;
  --darker-blue: #060b18;
  --medium-blue: #1e293b;
  --light-blue: #334155;
  --text-primary: #f8fafc;
  --text-secondary: #94a3b8;
  --text-muted: #64748b;
  --border-color: #334155;
  --glow-blue: 0 0 10px rgba(0, 198, 255, 0.5), 0 0 20px rgba(0, 198, 255, 0.2);
  --glow-purple: 0 0 10px rgba(123, 44, 249, 0.5),
    0 0 20px rgba(123, 44, 249, 0.2);
  --glow-pink: 0 0 10px rgba(244, 63, 94, 0.5), 0 0 20px rgba(244, 63, 94, 0.2);
  --glow-green: 0 0 10px rgba(0, 255, 148, 0.5), 0 0 20px rgba(0, 255, 148, 0.2);
}

/* Reset and Global Styles */
* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
}

body {
  font-family: "Rajdhani", sans-serif;
  background-color: var(--darker-blue);
  color: var(--text-primary);
  line-height: 1.5;
}

a {
  color: var(--neon-blue);
  text-decoration: none;
  transition: all 0.3s ease;
}

a:hover {
  color: var(--neon-purple);
  text-shadow: var(--glow-blue);
}

button {
  font-family: "Rajdhani", sans-serif;
  cursor: pointer;
  border: none;
  outline: none;
}

.neon-theme {
  background-color: var(--darker-blue);
  min-height: 100vh;
  display: flex;
  flex-direction: column;
}

/* Header Styles */
.site-header {
  background-color: var(--dark-blue);
  border-bottom: 1px solid var(--border-color);
  padding: 0.75rem 1.5rem;
  display: flex;
  justify-content: space-between;
  align-items: center;
  position: sticky;
  top: 0;
  z-index: 100;
}

.logo {
  display: flex;
  align-items: center;
}

.logo-link {
  text-decoration: none;
}

.logo-text {
  font-size: 1.75rem;
  font-weight: 700;
  background: linear-gradient(90deg, var(--neon-blue), var(--neon-purple));
  -webkit-background-clip: text;
  background-clip: text;
  color: transparent;
  text-shadow: var(--glow-blue);
}

.logo-separator {
  margin: 0 0.15rem;
  color: var(--neon-pink);
  text-shadow: var(--glow-pink);
}

.header-actions {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.neon-btn {
  background-color: transparent;
  border: 1px solid;
  border-radius: 6px;
  padding: 0.5rem 1rem;
  font-weight: 600;
  display: flex;
  align-items: center;
  gap: 0.5rem;
  transition: all 0.3s ease;
}

.neon-btn.blue {
  border-color: var(--neon-blue);
  color: var(--neon-blue);
}

.neon-btn.blue:hover {
  background-color: rgba(0, 198, 255, 0.1);
  box-shadow: var(--glow-blue);
}

.neon-btn.green {
  border-color: var(--neon-green);
  color: var(--neon-green);
}

.neon-btn.green:hover {
  background-color: rgba(0, 255, 148, 0.1);
  box-shadow: var(--glow-green);
}

.neon-btn.icon-only {
  padding: 0.5rem;
  border-color: var(--border-color);
  color: var(--text-secondary);
}

.neon-btn.icon-only:hover {
  background-color: var(--light-blue);
  color: var(--text-primary);
}

.user-menu {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 0.5rem 1rem;
  border-radius: 6px;
  background-color: rgba(123, 44, 249, 0.1);
  border: 1px solid var(--neon-purple);
  color: var(--text-primary);
  cursor: pointer;
  transition: all 0.3s ease;
}

.user-menu:hover {
  box-shadow: var(--glow-purple);
}

.user-avatar {
  width: 28px;
  height: 28px;
  border-radius: 50%;
  background: linear-gradient(135deg, var(--neon-purple), var(--neon-pink));
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 600;
}

.dropdown-icon {
  font-size: 10px;
  opacity: 0.7;
}

/* Layout Styles */
.layout {
  display: flex;
  flex: 1;
}

/* Sidebar Styles */
.sidebar {
  width: 80px;
  background-color: var(--dark-blue);
  border-right: 1px solid var(--border-color);
  padding: 1.5rem 0;
  position: sticky;
  top: 61px; /* Header height */
  height: calc(100vh - 61px);
  overflow-y: auto;
}

.nav-menu {
  display: flex;
  flex-direction: column;
  gap: 1rem;
  align-items: center;
}

.nav-item {
  width: 48px;
  height: 48px;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  border-radius: 8px;
  transition: all 0.3s ease;
  position: relative;
  color: var(--text-secondary);
}

.nav-item:hover {
  background-color: var(--light-blue);
  color: var(--text-primary);
}

.nav-item.active {
  background-color: var(--neon-blue);
  color: white;
  box-shadow: var(--glow-blue);
}

.nav-item.active::before {
  content: "";
  position: absolute;
  left: -1.5rem;
  width: 3px;
  height: 24px;
  background-color: var(--neon-blue);
  border-radius: 0 3px 3px 0;
}

.nav-icon {
  font-size: 1.25rem;
  margin-bottom: 0.25rem;
}

.nav-text {
  font-size: 0.7rem;
  font-weight: 600;
}

/* Main Content Styles */
.main-content {
  flex: 1;
  padding: 1.5rem;
  overflow: hidden;
}

/* Breadcrumb Styles */
.breadcrumb {
  display: flex;
  align-items: center;
  margin-bottom: 1.5rem;
  font-size: 0.875rem;
  color: var(--text-muted);
}

.breadcrumb a {
  color: var(--text-secondary);
}

.breadcrumb a:hover {
  color: var(--neon-blue);
}

.breadcrumb-separator {
  margin: 0 0.5rem;
}

.breadcrumb-current {
  color: var(--text-primary);
}

/* Post Header Styles */
.post-header {
  margin-bottom: 1.5rem;
  padding-bottom: 1.5rem;
  border-bottom: 1px solid var(--border-color);
}

.post-title {
  font-size: 1.75rem;
  font-weight: 700;
  margin-bottom: 0.5rem;
  color: var(--neon-blue);
  text-shadow: var(--glow-blue);
}

.post-meta {
  font-size: 0.875rem;
  color: var(--text-secondary);
}

/* Loading Styles */
.loading {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 3rem 0;
  gap: 1rem;
}

.loading-spinner {
  width: 40px;
  height: 40px;
  border: 3px solid var(--border-color);
  border-top-color: var(--neon-blue);
  border-radius: 50%;
  animation: spin 1s infinite linear;
}

@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}

/* Post Content Styles */
.post-content {
  display: flex;
  background-color: var(--dark-blue);
  border-radius: 8px;
  overflow: hidden;
  margin-bottom: 2rem;
  border: 1px solid var(--border-color);
}

.author-info {
  padding: 1.5rem;
  width: 180px;
  border-right: 1px solid var(--border-color);
  display: flex;
  flex-direction: column;
  align-items: center;
  text-align: center;
}

.author-avatar {
  width: 80px;
  height: 80px;
  border-radius: 50%;
  background: linear-gradient(135deg, var(--neon-blue), var(--neon-purple));
  display: flex;
  align-items: center;
  justify-content: center;
  margin-bottom: 1rem;
  box-shadow: var(--glow-blue);
}

.avatar-letter {
  font-size: 2rem;
  font-weight: 700;
  color: white;
}

.author-name {
  font-weight: 600;
  margin-bottom: 0.5rem;
}

.author-role {
  font-size: 0.75rem;
  background-color: var(--medium-blue);
  padding: 0.25rem 0.75rem;
  border-radius: 4px;
  color: var(--text-secondary);
}

.post-body {
  flex: 1;
  padding: 1.5rem;
}

.post-body p {
  margin-bottom: 1.5rem;
  line-height: 1.7;
}

.post-image {
  margin: 1.5rem 0;
  border-radius: 8px;
  overflow: hidden;
  border: 1px solid var(--border-color);
}

.post-image img {
  width: 100%;
  display: block;
  max-height: 500px;
  object-fit: contain;
}

/* Post Actions */
.post-actions {
  display: flex;
  gap: 1rem;
  margin-top: 1.5rem;
}

.action-btn {
  padding: 0.5rem 1rem;
  border-radius: 6px;
  display: flex;
  align-items: center;
  gap: 0.5rem;
  transition: all 0.3s ease;
  font-weight: 600;
  font-size: 0.875rem;
}

.action-btn.like {
  background-color: rgba(244, 63, 94, 0.1);
  color: var(--neon-pink);
  border: 1px solid var(--neon-pink);
}

.action-btn.like:hover {
  background-color: rgba(244, 63, 94, 0.2);
  box-shadow: var(--glow-pink);
}

.action-btn.like.active {
  background-color: var(--neon-pink);
  color: white;
}

.action-btn.share,
.action-btn.report {
  background-color: var(--medium-blue);
  color: var(--text-secondary);
  border: 1px solid var(--border-color);
}

.action-btn.share:hover,
.action-btn.report:hover {
  background-color: var(--light-blue);
  color: var(--text-primary);
}

/* Comments Section */
.comments-section {
  margin-bottom: 2rem;
}

.section-title {
  font-size: 1.5rem;
  font-weight: 700;
  margin-bottom: 1.5rem;
  display: flex;
  align-items: center;
  color: var(--neon-purple);
  text-shadow: var(--glow-purple);
}

.comment-count {
  color: var(--text-secondary);
  font-size: 1rem;
  font-weight: normal;
  margin-left: 0.5rem;
}

/* Comments List */
.comments-list {
  margin-bottom: 2rem;
}

.comment-item {
  display: flex;
  gap: 1rem;
  padding: 1rem;
  background-color: var(--dark-blue);
  border: 1px solid var(--border-color);
  border-radius: 8px;
  margin-bottom: 1rem;
}

.comment-user-avatar {
  width: 40px;
  height: 40px;
  border-radius: 50%;
  background: linear-gradient(135deg, var(--neon-purple), var(--neon-pink));
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 600;
  color: white;
}

.comment-content {
  flex: 1;
}

.comment-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 0.5rem;
}

.comment-author {
  font-weight: 600;
}

.comment-time {
  font-size: 0.75rem;
  color: var(--text-muted);
}

.comment-text {
  line-height: 1.6;
}

/* Comment Form */
.comment-form {
  background-color: var(--dark-blue);
  border: 1px solid var(--border-color);
  border-radius: 8px;
  padding: 1.5rem;
}

.form-title {
  font-size: 1.25rem;
  font-weight: 600;
  margin-bottom: 1rem;
  color: var(--text-primary);
}

.form-input {
  margin-bottom: 1rem;
}

.comment-textarea {
  width: 100%;
  min-height: 100px;
  padding: 0.75rem;
  background-color: var(--medium-blue);
  border: 1px solid var(--border-color);
  border-radius: 6px;
  color: var(--text-primary);
  font-family: inherit;
  resize: vertical;
  transition: all 0.3s ease;
}

.comment-textarea:focus {
  outline: none;
  border-color: var(--neon-blue);
  box-shadow: var(--glow-blue);
}

.form-actions {
  display: flex;
  justify-content: flex-end;
}

.post-comment {
  min-width: 120px;
}

.post-comment:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

/* Related Discussions */
.related-discussions {
  margin-top: 2rem;
}

.discussion-list {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.discussion-item {
  display: flex;
  justify-content: space-between;
  gap: 1rem;
  padding: 1rem;
  background-color: var(--dark-blue);
  border: 1px solid var(--border-color);
  border-radius: 8px;
  transition: all 0.3s ease;
  cursor: pointer;
}

.discussion-item:hover {
  background-color: var(--medium-blue);
  border-color: var(--neon-blue);
  box-shadow: var(--glow-blue);
}

.discussion-info {
  flex: 1;
}

.discussion-title {
  font-size: 1rem;
  font-weight: 600;
  margin-bottom: 0.5rem;
  color: var(--neon-blue);
}

.discussion-preview {
  font-size: 0.875rem;
  color: var(--text-secondary);
  display: -webkit-box;
  -webkit-line-clamp: 2;
  line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.discussion-meta {
  flex-shrink: 0;
  text-align: right;
  min-width: 100px;
}

.discussion-date {
  font-size: 0.75rem;
  color: var(--neon-green);
  margin-bottom: 0.25rem;
}

.discussion-author {
  font-size: 0.75rem;
  color: var(--text-muted);
}

/* Responsive Styles */
@media (max-width: 768px) {
  .layout {
    flex-direction: column;
  }

  .sidebar {
    width: 100%;
    height: auto;
    position: static;
    padding: 0.75rem;
    border-right: none;
    border-bottom: 1px solid var(--border-color);
  }

  .nav-menu {
    flex-direction: row;
    justify-content: space-around;
  }

  .nav-item.active::before {
    display: none;
  }

  .post-content {
    flex-direction: column;
  }

  .author-info {
    width: 100%;
    border-right: none;
    border-bottom: 1px solid var(--border-color);
    padding: 1rem;
    flex-direction: row;
    justify-content: flex-start;
    text-align: left;
    gap: 1rem;
  }

  .post-actions {
    flex-wrap: wrap;
  }

  .discussion-item {
    flex-direction: column;
  }

  .discussion-meta {
    text-align: left;
  }

  .header-actions {
    display: none;
  }

  .user-menu {
    display: flex;
  }
}

@media (max-width: 480px) {
  .user-name {
    display: none;
  }
}

/* Add custom font */
@import url("https://fonts.googleapis.com/css2?family=Rajdhani:wght@300;400;500;600;700&display=swap");
</style>
