import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { environment } from '../../environments/environment';
import { Student, StudentDto } from '../models/student.model';
import { ApiResponse } from '../models/api-response.model';

@Injectable({ providedIn: 'root' })
export class StudentService {
  private base = `${environment.apiUrl}/Student`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<Student[]> {
    return this.http
      .get<ApiResponse<Student[]>>(`${this.base}/All`)
      .pipe(map(res => res.data ?? []));
  }

  getById(id: number): Observable<Student> {
    return this.http
      .get<ApiResponse<Student>>(`${this.base}/Get/${id}`)
      .pipe(map(res => res.data));
  }

  create(student: StudentDto): Observable<Student> {
    return this.http
      .post<ApiResponse<Student>>(`${this.base}/createstudent`, student)
      .pipe(map(res => res.data));
  }

  update(student: Student): Observable<ApiResponse<null>> {
    // updaterecord returns NoContent in an APIResponse envelope (data = null)
    return this.http.put<ApiResponse<null>>(`${this.base}/updaterecord`, student);
  }

  delete(id: number): Observable<boolean> {
    return this.http
      .delete<ApiResponse<boolean>>(`${this.base}/deletestudent/${id}`)
      .pipe(map(res => res.data));
  }
}