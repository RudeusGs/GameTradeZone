<script setup lang="ts">
import { ref, onMounted, watch } from "vue"; // Import watch
// import axios from "axios"; // Remove axios if not used elsewhere
import { useRoute } from "vue-router";
import forumApi from "../api/forums"; // Import forum API

// 📌 Get postId from route
const route = useRoute();
const postId = ref<string>(String(route.params.id)); // Use string for API consistency

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
  categoryId?: number; // Added categoryId to Post interface
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
const isLiked = ref<boolean>(false); // Consider fetching initial like status if needed
const isLikeProcessing = ref<boolean>(false); // Prevent multiple clicks
const editingCommentId = ref<number | null>(null); // Track which comment is being edited
const editedCommentContent = ref<string>(""); // Store the edited content
const isDeletingCommentId = ref<number | null>(null); // Track which comment is being deleted
const isSavingEdit = ref<boolean>(false); // Track if edit save is in progress
const relatedPosts = ref<Post[]>([]); // State for related posts
const isLoadingRelated = ref<boolean>(false); // Loading state for related posts

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

// 🟢 Fetch post details using forumApi
const fetchPost = async () => {
  try {
    isLoading.value = true;
    console.log("🔍 Fetching post with ID:", postId.value);
    const response = await forumApi.getPostById(postId.value);
    post.value = response.data.result;

    // Fetch related posts after getting the category
    if (post.value?.categoryId) {
      await fetchRelatedPosts(post.value.categoryId);
    }

    // TODO: Fetch initial like status for the current user here if possible
    // isLiked.value = response.data.result.isLikedByUser; // Example
  } catch (error) {
    console.error("❌ Error fetching post:", error);
    post.value = null; // Reset post on error
  } finally {
    isLoading.value = false;
  }
};

// 🟢 Fetch comments using forumApi
const fetchComments = async () => {
  try {
    console.log("📌 Current Post ID:", postId.value); // Debug postId
    console.log("🔍 Fetching comments for post ID:", postId.value);
    const response = await forumApi.getPostComments(postId.value);
    console.log("✅ Comments fetched:", response.data.result); // Log comments
    comments.value = response.data.result;
  } catch (error) {
    console.error("❌ Error fetching comments:", error);
    comments.value = []; // Reset comments on error
  }
};

// 📚 Fetch related posts based on category
const fetchRelatedPosts = async (categoryId: number | undefined) => {
  if (categoryId === undefined) {
    relatedPosts.value = []; // Clear if no category ID
    return;
  }

  isLoadingRelated.value = true;
  try {
    console.log("📚 Fetching related posts for category ID:", categoryId);
    const response = await forumApi.getPostsByCategory(String(categoryId));

    // Filter out the current post
    let allRelated = response.data.result.filter(
      (p: Post) => p.id !== Number(postId.value)
    );

    // Shuffle the array (Fisher-Yates shuffle)
    for (let i = allRelated.length - 1; i > 0; i--) {
      const j = Math.floor(Math.random() * (i + 1));
      [allRelated[i], allRelated[j]] = [allRelated[j], allRelated[i]];
    }

    // Take the first 3 (or fewer if less than 3 are available)
    relatedPosts.value = allRelated.slice(0, 3);

    console.log(
      "✅ Related posts fetched (randomized & limited):",
      relatedPosts.value
    );
  } catch (error) {
    console.error("❌ Error fetching related posts:", error);
    relatedPosts.value = []; // Clear on error
  } finally {
    isLoadingRelated.value = false;
  }
};
// ❤️ Toggle Like/Unlike post using forumApi
const toggleLike = async () => {
  if (isLikeProcessing.value || !post.value) return; // Prevent multiple clicks or action if post not loaded

  isLikeProcessing.value = true;
  const currentlyLiked = isLiked.value;

  try {
    if (currentlyLiked) {
      // Unlike the post
      await forumApi.unlikePost(postId.value);
      post.value.likesCount -= 1;
      isLiked.value = false;
    } else {
      // Like the post
      await forumApi.likePost(postId.value);
      post.value.likesCount += 1;
      isLiked.value = true;
    }
  } catch (error) {
    console.error(
      `❌ Error ${currentlyLiked ? "unliking" : "liking"} post:`,
      error
    );
    // Optional: Revert UI changes on error
    if (post.value) {
      post.value.likesCount += currentlyLiked ? 1 : -1; // Revert count
    }
    isLiked.value = currentlyLiked; // Revert like status
  } finally {
    isLikeProcessing.value = false;
  }
};

// Report post function (remains the same)
const reportPost = () => {
  alert("Report functionality will be implemented in a future update.");
};

// Share post function (remains the same)
const sharePost = () => {
  navigator.clipboard.writeText(window.location.href);
  alert("Link copied to clipboard!");
};

// ✍️ Submit new comment using forumApi
const submitComment = async () => {
  if (!newComment.value.trim()) return;

  try {
    isSubmitting.value = true;
    await forumApi.commentOnPost({
      postId: Number(postId.value),
      content: newComment.value,
    });
    newComment.value = "";
    await fetchComments(); // Refresh comments list
  } catch (error) {
    console.error("❌ Error submitting comment:", error);
  } finally {
    isSubmitting.value = false;
  }
};

// 🗑️ Delete comment using forumApi
const deleteComment = async (commentId: number) => {
  if (!confirm("Are you sure you want to delete this comment?")) {
    return;
  }
  isDeletingCommentId.value = commentId; // Indicate deletion in progress
  try {
    await forumApi.deleteComment(String(commentId)); // API expects string ID
    // Remove comment from local state immediately for better UX
    comments.value = comments.value.filter((c) => c.id !== commentId);
    // Optional: Show success message
  } catch (error) {
    console.error("❌ Error deleting comment:", error);
    // Optional: Show error message
  } finally {
    isDeletingCommentId.value = null; // Reset deletion state
  }
};

// ✏️ Start editing a comment
const startEditing = (comment: Comment) => {
  editingCommentId.value = comment.id;
  editedCommentContent.value = comment.content; // Pre-fill with current content
};

// ❌ Cancel editing
const cancelEditing = () => {
  editingCommentId.value = null;
  editedCommentContent.value = "";
};

// ✅ Save edited comment using forumApi
const saveEdit = async (commentId: number) => {
  if (!editedCommentContent.value.trim()) {
    alert("Comment cannot be empty.");
    return;
  }
  isSavingEdit.value = true;
  try {
    await forumApi.updateComment(String(commentId), {
      // API expects string ID
      content: editedCommentContent.value,
    });
    // Update local comment data
    const index = comments.value.findIndex((c) => c.id === commentId);
    if (index !== -1) {
      comments.value[index].content = editedCommentContent.value;
    }
    cancelEditing(); // Exit editing mode
    // Optional: Show success message
  } catch (error) {
    console.error("❌ Error updating comment:", error);
    // Optional: Show error message
  } finally {
    isSavingEdit.value = false;
  }
};

// 👀 Watch for route parameter changes to reload data
watch(
  () => route.params.id,
  async (newId) => {
    const currentId = postId.value;
    if (newId && String(newId) !== currentId) {
      console.log("🔄 Route ID changed, refetching data for post:", newId);
      postId.value = String(newId); // Update the reactive postId ref

      // Reset states before fetching new data
      post.value = null;
      comments.value = [];
      relatedPosts.value = [];
      isLiked.value = false; // Reset like status
      isLoading.value = true; // Show loading indicator
      isLoadingRelated.value = true;
      editingCommentId.value = null; // Cancel any ongoing edit
      editedCommentContent.value = "";

      // Scroll to top for better UX
      window.scrollTo(0, 0);

      // Refetch all data for the new post ID
      await fetchPost(); // This will call fetchRelatedPosts internally
      await fetchComments();
    }
  },
  { immediate: false } // Don't run immediately on mount, onMounted handles initial load
);

// 🔄 Call API when component mounts
onMounted(() => {
  fetchPost();
  fetchComments();
  // Optional: Fetch initial like status for the current user if needed
});
</script>
<template>
  <div class="neon-theme">
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
          <router-link
            v-if="post?.categoryId"
            :to="`/forums/category/${post?.categoryId}`"
            >Category</router-link
          >
          <!-- TODO: Make dynamic based on actual category name -->
          <span v-if="post?.categoryId" class="breadcrumb-separator">/</span>
          <span class="breadcrumb-current">{{
            post?.caption || (isLoading ? "Loading..." : "Post Not Found")
          }}</span>
        </div>

        <!-- Post Header -->
        <div class="post-header">
          <h1 class="post-title">
            {{ post?.caption || (isLoading ? "Loading..." : "") }}
          </h1>
          <div v-if="post" class="post-meta">
            Posted
            {{
              post.createdDate
                ? formatDate(post.createdDate)
                : "Loading date..."
            }}
            by
            {{ post.user?.fullName || post.user?.userName || "Unknown User" }}
          </div>
        </div>

        <!-- Loading State -->
        <div v-if="isLoading" class="loading">
          <div class="loading-spinner"></div>
          <span>Loading Post...</span>
        </div>

        <!-- Post Content -->
        <div v-else-if="post" class="post-content">
          <!-- Author Info -->
          <div class="author-info">
            <div class="author-avatar">
              <span class="avatar-letter">{{
                post.user?.fullName?.[0] || post.user?.userName?.[0] || "U"
              }}</span>
            </div>
            <div class="author-name">
              {{ post.user?.fullName || post.user?.userName || "Unknown User" }}
            </div>
            <div class="author-role">Member</div>
            <!-- Consider fetching role if available -->
          </div>

          <!-- Post Body -->
          <div class="post-body">
            <p v-html="post.content"></p>
            <!-- Use v-html if content can contain HTML -->

            <div v-if="post.imageUrl" class="post-image">
              <img :src="post.imageUrl" alt="Post Image" />
            </div>

            <!-- Post Actions -->
            <div class="post-actions">
              <button
                class="action-btn like"
                :class="{ active: isLiked }"
                @click="toggleLike"
                :disabled="isLikeProcessing"
              >
                <span class="action-icon">❤️</span>
                <span class="action-count">{{ post.likesCount }}</span>
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
        <div v-else class="error-message">
          Failed to load post details or post not found.
        </div>

        <!-- Comments Section -->
        <div v-if="post" class="comments-section">
          <h2 class="section-title">
            Comments<span class="comment-count">({{ comments.length }})</span>
          </h2>

          <!-- Comments List -->
          <div class="comments-list">
            <div v-if="!comments.length && !isLoading" class="no-comments">
              No comments yet. Be the first to comment!
            </div>
            <div
              v-for="comment in comments"
              :key="comment.id"
              class="comment-item"
            >
              <div class="comment-user">
                <div class="comment-user-avatar">
                  {{
                    comment.user?.fullName?.[0] ||
                    comment.user?.userName?.[0] ||
                    "U"
                  }}
                </div>
              </div>
              <div class="comment-content">
                <div class="comment-header">
                  <span class="comment-author">{{
                    comment.user?.fullName ||
                    comment.user?.userName ||
                    "Unknown User"
                  }}</span>
                  <span class="comment-time">{{
                    formatDate(comment.createdDate)
                  }}</span>
                </div>
                <!-- Editing View -->
                <div
                  v-if="editingCommentId === comment.id"
                  class="comment-edit-view"
                >
                  <textarea
                    v-model="editedCommentContent"
                    class="comment-edit-textarea"
                    rows="3"
                    :disabled="isSavingEdit"
                  ></textarea>
                  <div class="comment-edit-actions">
                    <button
                      class="neon-btn green small"
                      @click="saveEdit(comment.id)"
                      :disabled="isSavingEdit || !editedCommentContent.trim()"
                    >
                      {{ isSavingEdit ? "Saving..." : "Save" }}
                    </button>
                    <button
                      class="neon-btn gray small"
                      @click="cancelEditing"
                      :disabled="isSavingEdit"
                    >
                      Cancel
                    </button>
                  </div>
                </div>
                <!-- Normal View -->
                <div v-else class="comment-text">{{ comment.content }}</div>

                <!-- Comment Actions (Edit/Delete) - TODO: Add logic to show only for comment owner -->
                <div
                  v-if="editingCommentId !== comment.id"
                  class="comment-item-actions"
                >
                  <button
                    class="comment-action-btn edit"
                    @click="startEditing(comment)"
                    title="Edit Comment"
                  >
                    ✏️ Edit
                  </button>
                  <button
                    class="comment-action-btn delete"
                    @click="deleteComment(comment.id)"
                    :disabled="isDeletingCommentId === comment.id"
                    title="Delete Comment"
                  >
                    {{
                      isDeletingCommentId === comment.id
                        ? "Deleting..."
                        : "🗑️ Delete"
                    }}
                  </button>
                </div>
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
                :disabled="isSubmitting"
              ></textarea>
            </div>
            <div class="form-actions">
              <button
                class="neon-btn blue post-comment"
                :disabled="!newComment.trim() || isSubmitting"
                @click="submitComment"
              >
                {{ isSubmitting ? "Posting..." : "Post Comment" }}
              </button>
            </div>
          </div>
        </div>

        <!-- Related Discussions -->
        <div v-if="post" class="related-discussions">
          <h2 class="section-title">Related Discussions</h2>
          <div v-if="isLoadingRelated" class="loading-related">
            Loading related posts...
          </div>
          <div v-else-if="relatedPosts.length > 0" class="discussion-list">
            <router-link
              v-for="relatedPost in relatedPosts"
              :key="relatedPost.id"
              :to="`/post/${relatedPost.id}`"
              class="discussion-item"
            >
              <div class="discussion-info">
                <h3 class="discussion-title">{{ relatedPost.caption }}</h3>
                <!-- Optional: Add a preview if available -->
                <!-- <p class="discussion-preview">Preview...</p> -->
              </div>
              <div class="discussion-meta">
                <div class="discussion-date">
                  {{ formatDate(relatedPost.createdDate) }}
                </div>
                <div class="discussion-author">
                  by
                  {{
                    relatedPost.user?.fullName ||
                    relatedPost.user?.userName ||
                    "Unknown"
                  }}
                </div>
              </div>
            </router-link>
          </div>
          <div v-else class="no-related">
            No other discussions found in this category.
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

.neon-btn.blue:hover:not(:disabled) {
  background-color: rgba(0, 198, 255, 0.1);
  box-shadow: var(--glow-blue);
}

.neon-btn.green {
  border-color: var(--neon-green);
  color: var(--neon-green);
}

.neon-btn.green:hover:not(:disabled) {
  background-color: rgba(0, 255, 148, 0.1);
  box-shadow: var(--glow-green);
}

.neon-btn.icon-only {
  padding: 0.5rem;
  border-color: var(--border-color);
  color: var(--text-secondary);
}

.neon-btn.icon-only:hover:not(:disabled) {
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
  top: 0px; /* Header height */
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
  left: -1.5rem; /* Adjust if sidebar padding changes */
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
  overflow-y: auto; /* Allow scrolling if content overflows */
  max-height: calc(
    100vh - 61px
  ); /* Prevent content from pushing below viewport */
}

/* Breadcrumb Styles */
.breadcrumb {
  display: flex;
  align-items: center;
  margin-bottom: 1.5rem;
  font-size: 0.875rem;
  color: var(--text-muted);
  flex-wrap: wrap; /* Allow wrapping on smaller screens */
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
  color: var(--text-secondary);
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

/* Error Message */
.error-message {
  text-align: center;
  padding: 3rem;
  color: var(--neon-pink);
  font-style: italic;
  background-color: var(--dark-blue);
  border: 1px solid var(--border-color);
  border-radius: 8px;
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
  flex-shrink: 0; /* Prevent shrinking */
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
  word-break: break-word; /* Prevent long names from overflowing */
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
  min-width: 0; /* Allow shrinking */
}

.post-body p {
  margin-bottom: 1.5rem;
  line-height: 1.7;
  word-wrap: break-word; /* Ensure long words wrap */
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
  flex-wrap: wrap; /* Allow wrapping */
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

.action-btn.like:hover:not(:disabled) {
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

.action-btn.share:hover:not(:disabled),
.action-btn.report:hover:not(:disabled) {
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

.no-comments {
  text-align: center;
  padding: 1.5rem;
  color: var(--text-secondary);
  font-style: italic;
  background-color: var(--dark-blue);
  border: 1px solid var(--border-color);
  border-radius: 8px;
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

.comment-user {
  flex-shrink: 0; /* Prevent avatar shrinking */
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
  min-width: 0; /* Allow shrinking */
}

.comment-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 0.5rem;
  flex-wrap: wrap; /* Allow wrapping */
  gap: 0.5rem;
}

.comment-author {
  font-weight: 600;
  word-break: break-all; /* Break long author names */
}

.comment-time {
  font-size: 0.75rem;
  color: var(--text-muted);
  flex-shrink: 0; /* Prevent time from wrapping unnecessarily */
}

.comment-text {
  line-height: 1.6;
  word-wrap: break-word; /* Ensure long words wrap */
}

/* Comment Form */
.comment-form {
  background-color: var(--dark-blue);
  border: 1px solid var(--border-color);
  border-radius: 8px;
  padding: 1.5rem;
}

.comment-item-actions {
  margin-top: 0.75rem;
  display: flex;
  gap: 0.75rem;
}

.comment-action-btn {
  background: none;
  border: none;
  color: var(--text-muted);
  font-size: 0.8rem;
  cursor: pointer;
  transition: color 0.3s ease;
  padding: 0.25rem 0.5rem;
  border-radius: 4px;
}

.comment-action-btn:hover:not(:disabled) {
  color: var(--text-primary);
  background-color: var(--medium-blue);
}

.comment-action-btn.edit:hover:not(:disabled) {
  color: var(--neon-blue);
}
.comment-action-btn.delete:hover:not(:disabled) {
  color: var(--neon-pink);
}

.comment-action-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.comment-edit-view {
  margin-top: 0.5rem;
}

.comment-edit-textarea {
  width: 100%;
  padding: 0.5rem;
  background-color: var(--medium-blue);
  border: 1px solid var(--border-color);
  border-radius: 4px;
  color: var(--text-primary);
  font-family: inherit;
  resize: vertical;
  margin-bottom: 0.5rem;
}

.comment-edit-textarea:focus {
  outline: none;
  border-color: var(--neon-blue);
  box-shadow: var(--glow-blue);
}

.comment-edit-actions {
  display: flex;
  gap: 0.5rem;
}

.neon-btn.small {
  padding: 0.3rem 0.8rem;
  font-size: 0.8rem;
}

.neon-btn.gray {
  border-color: var(--border-color);
  color: var(--text-secondary);
}

.neon-btn.gray:hover:not(:disabled) {
  background-color: var(--light-blue);
  color: var(--text-primary);
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
  text-decoration: none; /* Remove underline from link */
}

.discussion-item:hover {
  background-color: var(--medium-blue);
  border-color: var(--neon-blue);
  box-shadow: var(--glow-blue);
}

.discussion-info {
  flex: 1;
  min-width: 0; /* Allow shrinking */
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
  word-break: break-word;
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

.loading-related,
.no-related {
  text-align: center;
  padding: 1.5rem;
  color: var(--text-secondary);
  font-style: italic;
  background-color: var(--dark-blue);
  border: 1px solid var(--border-color);
  border-radius: 8px;
}

/* Responsive Styles */
@media (max-width: 992px) {
  /* Adjust breakpoint if needed */
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
  .author-avatar {
    width: 50px;
    height: 50px;
    margin-bottom: 0;
  }
  .avatar-letter {
    font-size: 1.5rem;
  }
}

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
    overflow-y: visible; /* Remove scrollbar */
  }

  .nav-menu {
    flex-direction: row;
    justify-content: space-around;
  }

  .nav-item.active::before {
    display: none;
  }

  .main-content {
    max-height: none; /* Remove max-height */
    padding: 1rem; /* Reduce padding */
  }

  .discussion-item {
    flex-direction: column;
    gap: 0.5rem;
  }

  .discussion-meta {
    text-align: left;
    min-width: auto;
  }

  .header-actions {
    display: none; /* Hide header actions on smaller screens */
  }

  .user-menu {
    display: flex; /* Ensure user menu is visible */
  }
}

@media (max-width: 480px) {
  .user-name {
    display: none; /* Hide username in user menu */
  }
  .post-title {
    font-size: 1.5rem; /* Reduce title size */
  }
  .section-title {
    font-size: 1.25rem; /* Reduce section title size */
  }
  .comment-header {
    flex-direction: column; /* Stack author and time */
    align-items: flex-start;
  }
}

/* Add custom font */
@import url("https://fonts.googleapis.com/css2?family=Rajdhani:wght@300;400;500;600;700&display=swap");
</style>
