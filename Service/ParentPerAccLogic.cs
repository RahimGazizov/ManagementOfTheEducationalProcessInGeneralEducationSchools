using InformationSystemOfASchoolIducationalPortal.Data;
using InformationSystemOfASchoolIducationalPortal.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.Evaluation;
using Microsoft.EntityFrameworkCore;

namespace InformationSystemOfASchoolIducationalPortal.Service
{
    public class ParentPerAccLogic
    {
        private readonly AppDbContext _context;
        private readonly StudentPerAccLogic _studentPerAccLogic;
        public ParentPerAccLogic(AppDbContext context, StudentPerAccLogic studentPerAccLogic)
        {
            _context = context;
            _studentPerAccLogic = studentPerAccLogic;
        }
        public class OperationResult<T>
        {
            public bool Success { get; set; }
            public string? Message { get; set; }
            public T Data { get; set; }
            public static OperationResult<T> Ok(string? message, T data) => new OperationResult<T> { Success = true, Message = message, Data = data };
            public static OperationResult<T> Fail(string message) => new OperationResult<T> { Success = false, Message = message };
        }
        public async Task<OperationResult<ModelViewForParents>> ForIndexData(Parents parent)
        {
            try
            {
                string[] name = parent.User.FullName.Split(',');
                if (parent == null)
                    return OperationResult<ModelViewForParents>.Fail("Родитель не найден");
                var parentWithStudents = await _context.Parent
        .Include(p => p.Students)
        .ThenInclude(s => s.User)
        .Include(s => s.Students)
        .ThenInclude(s => s.Class)
        .FirstOrDefaultAsync(p => p.Id == parent.Id);
                if (parentWithStudents == null)
                    return OperationResult<ModelViewForParents>.Fail($"{name[1]} вам не назначили ученика");

                var children = parentWithStudents.Students
         .Select(s => new SelectListItem
         {
             Value = s.Id,
             Text = s.User.FullName
         })
         .ToList();
                List<StudentAvgScore> avgScores = new();
                var childIds = parentWithStudents.Students.Select(s => s.Id).ToList();
                var entries = await _context.JournalEntry
                    .Include(s => s.Journal)
                    .Where(s => childIds.Contains(s.StudentId))
                    .ToListAsync();
                foreach (var child in parentWithStudents.Students)
                {
                    var childGrades = entries
             .Where(e => e.StudentId == child.Id &&
                         e.Journal.ClassId == child.ClassId &&
                         e.Grade != null)
             .Select(e => e.Grade);

                    double avg = childGrades.Any() ? childGrades.Average() ?? 0 : 0;

                    avgScores.Add(new StudentAvgScore
                    {
                        Student = child,
                        AvgScore = avg
                    });
                }
                var viewModel = new ModelViewForParents
                {
                    Parent = parent,
                    Childrens = children,
                    Students = avgScores,
                    AvgDynamicsInfo = await AvgScoreDimamicsChildren(parentWithStudents.Students)
                };
                return OperationResult<ModelViewForParents>.Ok(null, viewModel);
            }
            catch (Exception ex)
            {
                return OperationResult<ModelViewForParents>.Fail(ex.Message);
            }
        }
        public async Task<Dictionary<string, double>> AvgScoreDimamicsChildren(List<Students> students)
        {
            if (!students.Any())
                return new();
            var studentAvgDynamicsInfo = new Dictionary<string, double>();
            foreach (var student in students)
            {
                var avg = await _studentPerAccLogic.AvgScoreDimamics(student);
                studentAvgDynamicsInfo.Add(student.Id, avg);
            }
            return studentAvgDynamicsInfo;
        }
    }
}
