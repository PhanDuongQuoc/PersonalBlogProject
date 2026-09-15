import api from "@/boot/ApiGateway/axios";
import type {
  AdminProfile,
  UpdateAdminProfilePayload,
  AdminSkill,
  CreateSkillPayload,
  UpdateSkillPayload,
  AdminExperience,
  CreateExperiencePayload,
  UpdateExperiencePayload,
  AdminEducation,
  CreateEducationPayload,
  UpdateEducationPayload,
  AdminSkillsOverviewStats
} from "@/types/admin-profile";

export const adminProfileService = {
  // ==========================================
  // 1. Profile Service
  // ==========================================
  async getProfile(): Promise<AdminProfile> {
    const res = await api.get<AdminProfile>("/admin/profile");
    return res.data;
  },

  async updateProfile(payload: UpdateAdminProfilePayload): Promise<AdminProfile> {
    const res = await api.put<AdminProfile>("/admin/profile", payload);
    return res.data;
  },

  async getOverviewStats(): Promise<AdminSkillsOverviewStats> {
    const res = await api.get<AdminSkillsOverviewStats>("/admin/profile/stats");
    return res.data;
  },

  // ==========================================
  // 2. Skills Service
  // ==========================================
  async getSkills(): Promise<AdminSkill[]> {
    const res = await api.get<AdminSkill[]>("/admin/profile/skills");
    return res.data;
  },

  async createSkill(payload: CreateSkillPayload): Promise<AdminSkill> {
    const res = await api.post<AdminSkill>("/admin/profile/skills", payload);
    return res.data;
  },

  async updateSkill(id: number, payload: UpdateSkillPayload): Promise<AdminSkill> {
    const res = await api.put<AdminSkill>(`/admin/profile/skills/${id}`, payload);
    return res.data;
  },

  async deleteSkill(id: number): Promise<void> {
    await api.delete(`/admin/profile/skills/${id}`);
  },

  // ==========================================
  // 3. Experiences Service
  // ==========================================
  async getExperiences(): Promise<AdminExperience[]> {
    const res = await api.get<AdminExperience[]>("/admin/profile/experiences");
    return res.data;
  },

  async createExperience(payload: CreateExperiencePayload): Promise<AdminExperience> {
    const res = await api.post<AdminExperience>("/admin/profile/experiences", payload);
    return res.data;
  },

  async updateExperience(id: number, payload: UpdateExperiencePayload): Promise<AdminExperience> {
    const res = await api.put<AdminExperience>(`/admin/profile/experiences/${id}`, payload);
    return res.data;
  },

  async deleteExperience(id: number): Promise<void> {
    await api.delete(`/admin/profile/experiences/${id}`);
  },

  // ==========================================
  // 4. Educations Service
  // ==========================================
  async getEducations(): Promise<AdminEducation[]> {
    const res = await api.get<AdminEducation[]>("/admin/profile/educations");
    return res.data;
  },

  async createEducation(payload: CreateEducationPayload): Promise<AdminEducation> {
    const res = await api.post<AdminEducation>("/admin/profile/educations", payload);
    return res.data;
  },

  async updateEducation(id: number, payload: UpdateEducationPayload): Promise<AdminEducation> {
    const res = await api.put<AdminEducation>(`/admin/profile/educations/${id}`, payload);
    return res.data;
  },

  async deleteEducation(id: number): Promise<void> {
    await api.delete(`/admin/profile/educations/${id}`);
  }
};
