<script setup lang="ts">
import { ref, onMounted, watch } from "vue"; // Import watch
// import axios from "axios"; // Remove axios if not used elsewhere
import { useRoute, useRouter } from "vue-router";
import forumApi from "../api/forums"; // Import forum API

// 📌 Get postId from route
const route = useRoute();
const router = useRouter();
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
    router.push("/not-found"); // Redirect to not-found page
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
    <!-- Main Content - Facebook-style post card -->
    <div class="post-container">
      <!-- Breadcrumb - Simplified -->
      <div class="breadcrumb">
        <router-link to="/forums" class="breadcrumb-link">
          <i class="fas fa-home"></i> Forums
        </router-link>
        <span class="breadcrumb-separator">
          <i class="fas fa-chevron-right"></i>
        </span>
        <router-link
          v-if="post?.categoryId"
          :to="`/forums/category/${post?.categoryId}`"
          class="breadcrumb-link"
        >
          <i class="fas fa-folder"></i> Category
        </router-link>
        <span v-if="post?.categoryId" class="breadcrumb-separator">
          <i class="fas fa-chevron-right"></i>
        </span>
        <span class="breadcrumb-current">
          <i class="fas fa-file-alt"></i> Post
        </span>
      </div>

      <!-- Loading State -->
      <div v-if="isLoading" class="loading-card">
        <div class="loading-spinner"></div>
        <span>Loading...</span>
      </div>

      <!-- Post Card - Facebook Style -->
      <div v-else-if="post" class="post-card">
        <!-- Post Header -->
        <div class="post-header">
          <div class="post-author">
            <div class="author-avatar">
              <span class="avatar-letter">{{
                post.user?.fullName?.[0] || post.user?.userName?.[0] || "U"
              }}</span>
            </div>
            <div class="author-info">
              <div class="author-name">
                {{ post.user?.fullName || post.user?.userName || "Unknown User" }}
              </div>
              <div class="post-time">
                <i class="fas fa-clock"></i> {{ formatDate(post.createdDate) }}
              </div>
            </div>
          </div>
          <div class="post-options">
            <button class="post-option-btn" @click="reportPost" title="Report">
              <i class="fas fa-ellipsis-h"></i>
            </button>
          </div>
        </div>

        <!-- Post Caption -->
        <h3 class="post-caption">{{ post.caption }}</h3>

        <!-- Post Content -->
        <div class="post-content">
          <p v-html="post.content"></p>
        </div>

        <!-- Post Image (if available) -->
        <div v-if="post.imageUrl" class="post-image">
          <img :src="post.imageUrl" alt="Post Image" />
        </div>

        <!-- Post Stats -->
        <div class="post-stats">
          <div class="post-likes">
            <i class="fas fa-heart"></i> {{ post.likesCount }} likes
          </div>
          <div class="post-comments-count">
            <i class="fas fa-comment"></i> {{ comments.length }} comments
          </div>
        </div>

        <!-- Post Actions -->
        <div class="post-actions">
          <button
            class="action-btn like-btn"
            :class="{ active: isLiked }"
            @click="toggleLike"
            :disabled="isLikeProcessing"
          >
            <i class="fas" :class="isLiked ? 'fa-heart' : 'fa-heart'"></i>
            <span class="action-text">Like</span>
          </button>
          <button class="action-btn comment-btn">
            <i class="fas fa-comment"></i>
            <span class="action-text">Comment</span>
          </button>
          <button class="action-btn share-btn" @click="sharePost">
            <i class="fas fa-share"></i>
            <span class="action-text">Share</span>
          </button>
        </div>

        <!-- Comments Section -->
        <div class="comments-section">
          <!-- Comment Form -->
          <div class="comment-form">
            <div class="comment-avatar">
              <span class="avatar-letter">U</span>
            </div>
            <div class="comment-input-wrapper">
              <textarea
                v-model="newComment"
                placeholder="Write a comment..."
                class="comment-input"
                :disabled="isSubmitting"
              ></textarea>
              <button
                class="comment-submit"
                :disabled="!newComment.trim() || isSubmitting"
                @click="submitComment"
              >
                <i class="fas fa-paper-plane"></i>
              </button>
            </div>
          </div>

          <!-- Comments List -->
          <div class="comments-list">
            <div v-if="!comments.length && !isLoading" class="no-comments">
              <i class="fas fa-comment-slash"></i> No comments yet. Be the first to comment!
            </div>
            <div
              v-for="comment in comments"
              :key="comment.id"
              class="comment-item"
            >
              <div class="comment-avatar">
                <span class="avatar-letter">{{
                  comment.user?.fullName?.[0] ||
                  comment.user?.userName?.[0] ||
                  "U"
                }}</span>
              </div>
              <div class="comment-content">
                <!-- Normal View -->
                <div v-if="editingCommentId !== comment.id" class="comment-bubble">
                  <div class="comment-author">
                    {{ comment.user?.fullName || comment.user?.userName || "Unknown" }}
                  </div>
                  <div class="comment-text">{{ comment.content }}</div>
                  <div class="comment-meta">
                    <span class="comment-time">
                      <i class="fas fa-clock"></i> {{ formatDate(comment.createdDate) }}
                    </span>
                    <div class="comment-actions">
                      <button
                        class="comment-action-link"
                        @click="startEditing(comment)"
                        title="Edit Comment"
                      >
                        <i class="fas fa-edit"></i> Edit
                      </button>
                      <button
                        class="comment-action-link"
                        @click="deleteComment(comment.id)"
                        :disabled="isDeletingCommentId === comment.id"
                        title="Delete Comment"
                      >
                        <i class="fas fa-trash-alt"></i> 
                        {{ isDeletingCommentId === comment.id ? "Deleting..." : "Delete" }}
                      </button>
                    </div>
                  </div>
                </div>

                <!-- Editing View -->
                <div
                  v-else
                  class="comment-edit-view"
                >
                  <textarea
                    v-model="editedCommentContent"
                    class="comment-edit-textarea"
                    rows="2"
                    :disabled="isSavingEdit"
                  ></textarea>
                  <div class="comment-edit-actions">
                    <button
                      class="edit-btn save"
                      @click="saveEdit(comment.id)"
                      :disabled="isSavingEdit || !editedCommentContent.trim()"
                    >
                      <i class="fas fa-check"></i>
                      {{ isSavingEdit ? "Saving..." : "Save" }}
                    </button>
                    <button
                      class="edit-btn cancel"
                      @click="cancelEditing"
                      :disabled="isSavingEdit"
                    >
                      <i class="fas fa-times"></i>
                      Cancel
                    </button>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
      <div v-else class="error-card">
        <i class="fas fa-exclamation-triangle"></i> Post not found or failed to load.
      </div>

      <!-- Related Posts - Compact Card Style -->
      <div v-if="post && relatedPosts.length > 0" class="related-posts">
        <h3 class="related-title">
          <i class="fas fa-link"></i> Related Posts
        </h3>
        <div class="related-list">
          <router-link
            v-for="relatedPost in relatedPosts"
            :key="relatedPost.id"
            :to="`/post/${relatedPost.id}`"
            class="related-item"
          >
            <div class="related-content">
              <h4 class="related-caption">
                <i class="fas fa-file-alt"></i> {{ relatedPost.caption }}
              </h4>
              <div class="related-meta">
                <span class="related-author">
                  <i class="fas fa-user"></i>
                  {{ relatedPost.user?.fullName || relatedPost.user?.userName || "Unknown" }}
                </span>
                <span class="related-time">
                  <i class="fas fa-clock"></i> {{ formatDate(relatedPost.createdDate) }}
                </span>
              </div>
            </div>
          </router-link>
        </div>
      </div>
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

/* FontAwesome Import */
@import url('https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0-beta3/css/all.min.css');

/* Facebook-style Post Container */
.post-container {
  max-width: 600px;
  margin: 0 auto;
  padding: 1rem;
}

/* Breadcrumb - Styled with icons */
.breadcrumb {
  display: flex;
  align-items: center;
  margin-bottom: 1rem;
  font-size: 0.75rem;
  color: var(--text-muted);
  background-color: var(--dark-blue);
  padding: 0.5rem 0.75rem;
  border-radius: 8px;
  border: 1px solid var(--border-color);
}

.breadcrumb-link {
  color: var(--text-secondary);
  display: flex;
  align-items: center;
  gap: 0.25rem;
}

.breadcrumb-separator {
  margin: 0 0.25rem;
  color: var(--text-muted);
  font-size: 0.6rem;
}

.breadcrumb-current {
  color: var(--text-primary);
  display: flex;
  align-items: center;
  gap: 0.25rem;
}

/* Loading Card */
.loading-card {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 2rem;
  background-color: var(--dark-blue);
  border-radius: 8px;
  border: 1px solid var(--border-color);
  margin-bottom: 1rem;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
}

.loading-spinner {
  width: 30px;
  height: 30px;
  border: 3px solid var(--border-color);
  border-top-color: var(--neon-blue);
  border-radius: 50%;
  animation: spin 1s infinite linear;
  margin-bottom: 0.5rem;
  box-shadow: var(--glow-blue);
}

@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}

/* Error Card */
.error-card {
  padding: 1.5rem;
  background-color: var(--dark-blue);
  border-radius: 8px;
  border: 1px solid var(--neon-pink);
  color: var(--neon-pink);
  text-align: center;
  margin-bottom: 1rem;
  box-shadow: var(--glow-pink);
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
}

/* Post Card - Facebook Style */
.post-card {
  background-color: var(--dark-blue);
  border-radius: 8px;
  border: 1px solid var(--border-color);
  overflow: hidden;
  margin-bottom: 1rem;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.2);
}

/* Post Header */
.post-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0.75rem 1rem;
  border-bottom: 1px solid var(--border-color);
  background: linear-gradient(to right, var(--dark-blue), var(--medium-blue));
}

.post-author {
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

.author-avatar {
  width: 40px;
  height: 40px;
  border-radius: 50%;
  background: linear-gradient(135deg, var(--neon-blue), var(--neon-purple));
  display: flex;
  align-items: center;
  justify-content: center;
  box-shadow: var(--glow-blue);
  border: 2px solid rgba(255, 255, 255, 0.1);
}

.avatar-letter {
  font-size: 1.25rem;
  font-weight: 700;
  color: white;
}

.author-info {
  display: flex;
  flex-direction: column;
}

.author-name {
  font-weight: 600;
  font-size: 0.9rem;
  color: var(--neon-blue);
  text-shadow: var(--glow-blue);
}

.post-time {
  font-size: 0.75rem;
  color: var(--text-muted);
  display: flex;
  align-items: center;
  gap: 0.25rem;
}

.post-options {
  display: flex;
}

.post-option-btn {
  background: none;
  color: var(--text-secondary);
  font-size: 1rem;
  padding: 0.25rem 0.5rem;
  border-radius: 4px;
  transition: all 0.3s ease;
}

.post-option-btn:hover {
  background-color: var(--medium-blue);
  color: var(--text-primary);
  box-shadow: var(--glow-blue);
}

/* Post Caption */
.post-caption {
  padding: 0.75rem 1rem 0.5rem;
  font-size: 1.1rem;
  font-weight: 600;
  color: var(--neon-blue);
  text-shadow: var(--glow-blue);
  border-bottom: 1px solid rgba(0, 198, 255, 0.2);
  margin-bottom: 0.5rem;
}

/* Post Content */
.post-content {
  padding: 0 1rem 0.75rem;
  font-size: 0.9rem;
  line-height: 1.5;
}

.post-content p {
  margin-bottom: 0.5rem;
}

/* Post Image */
.post-image {
  width: 100%;
  max-height: 500px;
  overflow: hidden;
  margin-bottom: 0.5rem;
  border-top: 1px solid var(--border-color);
  border-bottom: 1px solid var(--border-color);
  position: relative;
}

.post-image img {
  width: 100%;
  object-fit: contain;
  display: block;
}

.post-image::after {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: linear-gradient(to bottom, 
    rgba(15, 23, 42, 0.1) 0%, 
    rgba(15, 23, 42, 0) 20%, 
    rgba(15, 23, 42, 0) 80%, 
    rgba(15, 23, 42, 0.1) 100%);
  pointer-events: none;
}

/* Post Stats */
.post-stats {
  display: flex;
  justify-content: space-between;
  padding: 0.5rem 1rem;
  border-top: 1px solid var(--border-color);
  border-bottom: 1px solid var(--border-color);
  background-color: rgba(30, 41, 59, 0.4);
  font-size: 0.8rem;
  color: var(--text-secondary);
}

.post-likes, .post-comments-count {
  display: flex;
  align-items: center;
  gap: 0.25rem;
}

.post-likes i {
  color: var(--neon-pink);
}

.post-comments-count i {
  color: var(--neon-blue);
}

/* Post Actions */
.post-actions {
  display: flex;
  padding: 0.5rem;
  background-color: var(--medium-blue);
}

.action-btn {
  flex: 1;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
  padding: 0.75rem 0.5rem;
  background: none;
  color: var(--text-secondary);
  transition: all 0.3s ease;
  font-size: 0.9rem;
  border-radius: 4px;
  margin: 0 0.25rem;
}

.action-btn:hover:not(:disabled) {
  background-color: rgba(51, 65, 85, 0.7);
  color: var(--text-primary);
  transform: translateY(-2px);
}

.action-btn i {
  font-size: 1.1rem;
}

.like-btn.active {
  color: var(--neon-pink);
}

.like-btn.active i {
  color: var(--neon-pink);
  text-shadow: var(--glow-pink);
}

.comment-btn:hover i {
  color: var(--neon-blue);
  text-shadow: var(--glow-blue);
}

.share-btn:hover i {
  color: var(--neon-green);
  text-shadow: var(--glow-green);
}

.action-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

/* Comments Section */
.comments-section {
  padding: 0.75rem 1rem;
  background-color: rgba(15, 23, 42, 0.7);
  border-top: 1px solid var(--border-color);
}

/* Comment Form */
.comment-form {
  display: flex;
  gap: 0.75rem;
  margin-bottom: 1rem;
  background-color: var(--medium-blue);
  padding: 0.75rem;
  border-radius: 8px;
  border: 1px solid var(--border-color);
}

.comment-avatar {
  width: 32px;
  height: 32px;
  border-radius: 50%;
  background: linear-gradient(135deg, var(--neon-purple), var(--neon-pink));
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
  border: 1px solid rgba(255, 255, 255, 0.1);
}

.comment-input-wrapper {
  position: relative;
  flex: 1;
}

.comment-input {
  width: 100%;
  padding: 0.5rem 3rem 0.5rem 0.75rem;
  background-color: var(--dark-blue);
  border: 1px solid var(--border-color);
  border-radius: 20px;
  color: var(--text-primary);
  font-family: inherit;
  font-size: 0.85rem;
  resize: none;
  min-height: 36px;
  max-height: 80px;
  overflow-y: auto;
}

.comment-input:focus {
  outline: none;
  border-color: var(--neon-blue);
  box-shadow: var(--glow-blue);
}

.comment-submit {
  position: absolute;
  right: 0.5rem;
  top: 50%;
  transform: translateY(-50%);
  background-color: var(--neon-blue);
  color: white;
  border-radius: 50%;
  width: 28px;
  height: 28px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 0.75rem;
  opacity: 0.9;
  transition: all 0.3s ease;
}

.comment-submit:hover:not(:disabled) {
  opacity: 1;
  box-shadow: var(--glow-blue);
  transform: translateY(-50%) scale(1.1);
}

.comment-submit:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

/* Comments List */
.comments-list {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.no-comments {
  text-align: center;
  padding: 0.75rem;
  color: var(--text-secondary);
  font-style: italic;
  font-size: 0.85rem;
  background-color: var(--dark-blue);
  border-radius: 8px;
  border: 1px dashed var(--border-color);
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
}

.comment-item {
  display: flex;
  gap: 0.75rem;
}

.comment-bubble {
  background-color: var(--medium-blue);
  border-radius: 18px;
  padding: 0.75rem;
  position: relative;
  max-width: 100%;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
  border: 1px solid var(--border-color);
}

.comment-author {
  font-weight: 600;
  font-size: 0.85rem;
  margin-bottom: 0.25rem;
  color: var(--neon-purple);
}

.comment-text {
  font-size: 0.85rem;
  word-break: break-word;
  margin-bottom: 0.5rem;
}

.comment-meta {
  display: flex;
  justify-content: space-between;
  align-items: center;
  font-size: 0.75rem;
  border-top: 1px solid rgba(51, 65, 85, 0.5);
  padding-top: 0.5rem;
}

.comment-time {
  color: var(--text-muted);
  display: flex;
  align-items: center;
  gap: 0.25rem;
}

.comment-actions {
  display: flex;
  gap: 0.75rem;
}

.comment-action-link {
  background: none;
  color: var(--text-secondary);
  font-size: 0.75rem;
  padding: 0.1rem 0.3rem;
  border-radius: 4px;
  transition: all 0.3s ease;
  display: flex;
  align-items: center;
  gap: 0.25rem;
}

.comment-action-link:hover:not(:disabled) {
  color: var(--neon-blue);
  background-color: rgba(0, 198, 255, 0.1);
  text-shadow: var(--glow-blue);
}

.comment-action-link:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

/* Comment Edit View */
.comment-edit-view {
  width: 100%;
  background-color: var(--medium-blue);
  border-radius: 8px;
  padding: 0.75rem;
  border: 1px solid var(--border-color);
}

.comment-edit-textarea {
  width: 100%;
  padding: 0.5rem;
  background-color: var(--dark-blue);
  border: 1px solid var(--border-color);
  border-radius: 8px;
  color: var(--text-primary);
  font-family: inherit;
  font-size: 0.85rem;
  resize: none;
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
  justify-content: flex-end;
}

.edit-btn {
  padding: 0.25rem 0.75rem;
  border-radius: 4px;
  font-size: 0.75rem;
  transition: all 0.3s ease;
  display: flex;
  align-items: center;
  gap: 0.25rem;
}

.edit-btn.save {
  background-color: var(--neon-green);
  color: var(--dark-blue);
}

.edit-btn.save:hover:not(:disabled) {
  box-shadow: var(--glow-green);
  transform: translateY(-2px);
}

.edit-btn.cancel {
  background-color: var(--medium-blue);
  color: var(--text-secondary);
  border: 1px solid var(--border-color);
}

.edit-btn.cancel:hover:not(:disabled) {
  background-color: var(--light-blue);
  color: var(--text-primary);
}

.edit-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

/* Related Posts */
.related-posts {
  margin-top: 1.5rem;
  background-color: var(--dark-blue);
  border-radius: 8px;
  padding: 1rem;
  border: 1px solid var(--border-color);
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
}

.related-title {
  font-size: 1rem;
  font-weight: 600;
  margin-bottom: 0.75rem;
  color: var(--neon-purple);
  text-shadow: var(--glow-purple);
  display: flex;
  align-items: center;
  gap: 0.5rem;
  border-bottom: 1px solid var(--border-color);
  padding-bottom: 0.5rem;
}

.related-list {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.related-item {
  display: block;
  padding: 0.75rem;
  background-color: var(--medium-blue);
  border: 1px solid var(--border-color);
  border-radius: 8px;
  transition: all 0.3s ease;
  text-decoration: none;
  color: var(--text-primary);
}

.related-item:hover {
  border-color: var(--neon-blue);
  box-shadow: var(--glow-blue);
  transform: translateY(-2px);
}

.related-caption {
  font-size: 0.9rem;
  font-weight: 600;
  margin-bottom: 0.5rem;
  color: var(--neon-blue);
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.related-meta {
  display: flex;
  justify-content: space-between;
  font-size: 0.75rem;
  color: var(--text-muted);
}

.related-author, .related-time {
  display: flex;
  align-items: center;
  gap: 0.25rem;
}

/* Responsive Styles */
@media (max-width: 768px) {
  .post-container {
    padding: 0.75rem;
  }
  
  .post-caption {
    font-size: 1rem;
  }
}

@media (max-width: 480px) {
  .post-header {
    padding: 0.5rem 0.75rem;
  }
  
  .author-avatar {
    width: 32px;
    height: 32px;
  }
  
  .avatar-letter {
    font-size: 1rem;
  }
  
  .post-caption {
    padding: 0.5rem 0.75rem 0.25rem;
  }
  
  .post-content {
    padding: 0 0.75rem 0.5rem;
  }
  
  .comments-section {
    padding: 0.5rem 0.75rem;
  }
  
  .related-meta {
    flex-direction: column;
    gap: 0.25rem;
  }
}

/* Add custom font */
@import url("https://fonts.googleapis.com/css2?family=Rajdhani:wght@300;400;500;600;700&display=swap");
</style>