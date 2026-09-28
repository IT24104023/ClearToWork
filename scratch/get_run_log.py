import urllib.request
import re

url = "https://github.com/IT24104023/ClearToWork/actions/runs/36279074318"
req = urllib.request.Request(url, headers={'User-Agent': 'Mozilla/5.0'})
html = urllib.request.urlopen(req).read().decode('utf-8')

job_urls = re.findall(r'href="(/IT24104023/ClearToWork/actions/runs/36279074318/job/\d+)"', html)
print("Job URLs:", list(dict.fromkeys(job_urls)))

for ju in list(dict.fromkeys(job_urls)):
    full_url = "https://github.com" + ju
    jhtml = urllib.request.urlopen(urllib.request.Request(full_url, headers={'User-Agent': 'Mozilla/5.0'})).read().decode('utf-8')
    print("\n--- JOB:", ju, "---")
    # Search for CS errors or 'error' lines
    lines = [l.strip() for l in jhtml.splitlines() if 'error' in l.lower() or 'failed' in l.lower()]
    for l in lines[:20]:
        if len(l) < 200:
            print("  ", l)
