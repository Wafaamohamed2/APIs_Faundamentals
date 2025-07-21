namespace APIs_Faundamentals.Repository
{
    // Generic repository class for handling data operations for various entities .. to separate data layer 
    public class GenericRepos<TEntity> where TEntity : class
    {
        private readonly Models.PracticContext _context;
        
        public GenericRepos(Models.PracticContext context)
        {
            _context = context;
        }
      public List<TEntity> SelectAll()
      {
           return _context.Set<TEntity>().ToList();

      }

      public TEntity SelectById(int id)
      {
            return _context.Set<TEntity>().Find(id);
      }
      
         public void Add(TEntity entity)
        {
                _context.Set<TEntity>().Add(entity);
                
        }
       
        public void Update(TEntity entity)
        {
                _context.Set<TEntity>().Update(entity);
                
        }

        public void Delete(int id)
        {
            var entity = _context.Set<TEntity>().Find(id);
            if (entity != null)
            {
                _context.Set<TEntity>().Remove(entity);
            }
        }
        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
