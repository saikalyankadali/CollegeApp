export interface Student {
  id: number;
  studentName: string;
  email: string;
  phone: string;
  dob: string;
  departmentId: number | null;
}

export type StudentDto = Omit<Student, 'id'> & { id?: number };